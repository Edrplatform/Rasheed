# EDR Agent: Technical Documentation
## Comprehensive System Architecture and Implementation Guide

---

## Executive Summary

This document provides a complete technical overview of the EDR Agent system, a high-performance security monitoring application designed to transform raw Sysmon telemetry data into standardized Elastic Common Schema (ECS) format. The system implements a sophisticated producer-consumer architecture utilizing bounded channels for reliable, scalable event processing with backpressure management and graceful degradation capabilities.

The primary objective of this system is to establish a robust data normalization pipeline that converts heterogeneous Sysmon event formats into a unified ECS representation, enabling streamlined security analysis, threat detection, and compliance reporting across enterprise environments.

---

## System Architecture Overview

### Core Design Philosophy

The EDR Agent employs a producer-consumer pattern as its foundational architecture, leveraging the System.Threading.Channels framework to establish a bounded buffer between data collection and processing stages. This architectural decision addresses several critical requirements in high-volume security telemetry processing: memory safety through capacity constraints, operational reliability through backpressure mechanisms, and performance optimization through batch processing strategies.

The system architecture separates concerns across three primary operational domains: collection, transformation, and persistence. This separation enables independent scaling of each component based on workload characteristics and resource availability.

### Architectural Components

The collection layer operates as the producer in the pipeline, continuously monitoring Sysmon event sources and enqueuing structured event objects into a bounded channel. This channel implements a wait-based full mode strategy, ensuring that no events are dropped when processing capacity is temporarily exceeded. Instead, the collection component blocks until capacity becomes available, providing deterministic behavior under load.

The transformation layer consists of a configurable worker pool that consumes events from the channel, applies event-type-specific mapping logic to convert Sysmon structures into ECS format, and stages the normalized results for batch writing. The worker pool architecture enables horizontal scaling through configuration adjustments without code modifications.

The persistence layer implements batch writing strategies for both raw and normalized event streams. By accumulating events into memory buffers and flushing at configurable intervals or size thresholds, the system minimizes disk I/O operations while maintaining acceptable latency characteristics. Separate writers handle raw Sysmon preservation and normalized ECS output, supporting both archival and operational use cases.

### Data Flow Sequence

The complete data flow progresses through five distinct stages. Initially, the Sysmon collector component monitors system event sources and constructs typed event objects representing process creation, network connections, file operations, registry modifications, and DNS queries. These structured objects are then enqueued into the bounded channel, where backpressure mechanisms activate if downstream processing capacity is insufficient.

Consumer threads operating in the worker pool dequeue events from the channel and route them to appropriate mapping functions based on event type identifiers. The mapping layer extracts relevant fields from the Sysmon structure, applies transformations such as timestamp formatting and user principal parsing, and populates corresponding ECS schema fields. Mapped events accumulate in memory buffers until batch size thresholds or flush intervals are reached.

Upon reaching configured thresholds, batch writers serialize accumulated events to JSONL format and append them to designated output files using asynchronous I/O operations. The raw batch writer preserves original Sysmon telemetry for archival purposes, while the normalized batch writer outputs standardized ECS documents for downstream security analytics platforms.

During graceful shutdown sequences initiated by cancellation signals, the collector completes its current collection cycle and invokes the channel writer completion method. This signals to all consumers that no additional events will arrive. Consumer threads continue processing remaining channel contents, flush any partial batches to disk, and properly close file handles before terminating. This approach ensures data integrity and prevents truncated output files during application shutdown.

---

## Component Specifications

### Data Models

The SysmonEvent model defines the structure of raw telemetry captured from Windows system monitoring. This model includes temporal metadata through a timestamp field, event classification via an integer event identifier, host identification through a computer name field, and a nested data structure containing event-specific attributes. The nested data object accommodates varying field sets across different event types, including process execution paths, process identifiers, command line arguments, network addresses and ports, file paths, registry keys, and DNS query strings.

The EcsEvent model represents the normalized output conforming to Elastic Common Schema specifications. This structure includes a standardized timestamp field with the at-symbol prefix convention, event classification fields for code, category, and action, host identification fields, process metadata including executable paths, process names, and identifiers, user context with separated domain and name components, network protocol and endpoint information, file system paths and names, registry paths and values, and DNS query details. This standardized schema enables consistent querying and analysis across heterogeneous security data sources.

### Collection Component

The SysmonCollector component serves as the primary data acquisition interface, responsible for capturing system events and delivering them to the processing pipeline. In the current architecture, this component no longer performs direct file I/O operations. Instead, it constructs SysmonEvent objects from monitored sources and enqueues them into the bounded channel instance provided during initialization.

The collector implements cancellation token monitoring to support graceful shutdown semantics. Upon receiving a cancellation request, the collector completes its current acquisition cycle, ensures all pending events are successfully enqueued, and invokes the channel writer completion method to signal downstream consumers that the event stream has concluded.

The bounded channel configuration with wait-based full mode ensures that when channel capacity is exhausted, the collector blocks rather than dropping events. This design prioritizes data completeness over maximum throughput, making it appropriate for security telemetry where event loss is unacceptable.

### Parsing Utilities

The SysmonParser component provides utility functions for reading and deserializing JSONL formatted event files. This component supports scenarios where event ingestion occurs from file sources rather than real-time system monitoring. The parser reads the input file line by line, deserializes each JSON object into a SysmonEvent instance using the System.Text.Json serializer, and yields an enumerable sequence suitable for batch processing or streaming scenarios.

Error handling within the parser adopts a continue-on-error strategy, logging deserialization failures without terminating the overall parsing operation. This approach ensures that corrupted or malformed records do not prevent processing of subsequent valid events in the input stream.

### Mapping Logic

The SysmonToEcsMapper component implements the core transformation logic that converts event-specific Sysmon structures into standardized ECS representations. This component employs a routing mechanism based on event identifier values, dispatching each event to a specialized mapping function corresponding to its type.

For process creation events identified by event identifier one, the mapper extracts the executable path, process identifier, command line arguments, and user context. It performs path parsing to extract the process name from the full executable path, splits the user principal into domain and name components, converts the process identifier string to an integer, and populates the corresponding ECS process and user fields.

Network connection events identified by event identifier three receive mapping to ECS network and endpoint schemas. The mapper transfers protocol information, source and destination IP addresses and port numbers to the appropriate ECS network fields, while preserving process context from the originating executable.

File creation events identified by event identifier eleven map to ECS file schema structures. The mapper extracts the target filename, performs path parsing to separate directory and filename components, and populates file path and name fields alongside process context.

Registry modification events identified by event identifier thirteen map to ECS registry schema fields. The mapper transfers the target registry object path and any associated values or data to the registry-specific ECS fields.

DNS query events identified by event identifier twenty-two map to ECS network and DNS schema structures. The mapper transfers the query name to the DNS question field, response addresses to the DNS answers field, and query status indicators to the DNS response code field.

All mapping functions include timestamp normalization to ensure consistent temporal representation across event types. User context parsing splits compound domain and username strings on the backslash delimiter, handling both fully qualified and local account formats.

### Batch Writing Services

The RawBatchWriter component implements persistent storage for unmodified Sysmon telemetry. This service operates as a background consumer, continuously reading from the bounded channel and accumulating events into an in-memory buffer. When the buffer reaches the configured batch size threshold or when the configured flush interval elapses, the accumulated events are serialized to JSONL format and appended to the designated raw events file.

The writer employs asynchronous I/O operations through FileStream and StreamWriter interfaces, minimizing thread blocking during disk operations. JsonSerializerOptions instances are reused across serialization operations to reduce memory allocation overhead. During shutdown sequences, the writer monitors the cancellation token while continuing to process any remaining events in the channel and buffer, ensuring complete data persistence before termination.

The NormalizationWorkerPool component manages a configurable collection of worker tasks that consume events from the channel, apply ECS mapping transformations, and stage results for batch writing. The pool size is determined by the WorkerCount configuration parameter, enabling workload-appropriate concurrency levels. Each worker operates independently, preventing failures in one worker from affecting others in the pool.

Worker tasks retrieve events from the channel, invoke the SysmonToEcsMapper for transformation, and add resulting EcsEvent objects to a thread-safe staging buffer. The NormalizedBatchWriter component periodically flushes this staging buffer using the same batch size and flush interval semantics employed by the raw writer. This parallel batch writing architecture ensures that both raw preservation and normalized output maintain consistent performance characteristics.

Exception handling within workers captures and logs transformation errors without terminating the worker task itself. This resilient design ensures that malformed or unexpected event structures do not compromise overall system availability.

### Configuration Management

The AgentOptions component defines all user-configurable parameters governing system behavior. Critical configuration fields include BatchSize, which determines the number of events accumulated before triggering a write operation; FlushIntervalMs, which specifies the maximum duration in milliseconds between write operations regardless of batch size; ChannelCapacity, which defines the maximum number of events that can be queued in the bounded channel; and WorkerCount, which determines the number of concurrent normalization workers in the pool.

Additional configuration parameters specify file system paths for raw and normalized output files, enabling deployment-specific storage location management. The configuration structure supports serialization to and deserialization from JSON format, enabling persistent configuration storage in agent_config.json files.

Default values provide reasonable starting points for typical deployments, while production environments should adjust these parameters based on observed event rates, available memory, and disk I/O characteristics. Capacity planning should consider that channel capacity directly impacts memory usage, with each slot potentially holding a complete SysmonEvent object including all field data.

### Application Orchestration

The Program component serves as the application entry point, responsible for initialization, orchestration, and shutdown coordination. During startup, the component loads configuration from the agent_config.json file if present, falling back to default values otherwise. It constructs the bounded channel instance with capacity and behavior specified by the loaded configuration, instantiates all service components with appropriate dependencies, and initiates background tasks for collection and processing.

Command line argument parsing supports three operational modes. The collect-only mode activates the SysmonCollector and RawBatchWriter components while leaving normalization services inactive, supporting scenarios where raw event capture is required without immediate processing overhead. The normalize-only mode activates the normalization worker pool and normalized batch writer while leaving collection inactive, enabling dedicated processing nodes that consume from centralized raw event stores. The both mode activates all components simultaneously, providing integrated collection and normalization in a single process.

Cancellation token source management enables graceful shutdown through signal handling. When the application receives an interrupt signal such as Control-C, it invokes cancellation on the token source, waits for the collector to complete its shutdown sequence including channel writer completion, and then awaits all consumer tasks until they finish processing remaining channel contents and flush final batches to disk. This coordinated shutdown sequence ensures data integrity and prevents incomplete writes during application termination.

---

## Event Type Mapping Examples

### Process Creation Events

A Sysmon process creation event contains timestamp indicating when the process was initiated, event identifier value of one, hostname of the system where creation occurred, and a data structure including the full executable path such as C:\Windows\System32\notepad.exe, process identifier as a string value, command line arguments passed to the process, and user context in domain\username format.

The corresponding ECS representation includes a timestamp field formatted according to ECS temporal conventions, event code matching the original identifier, event category set to "process", event action set to "start", hostname transferred directly, process executable path preserved from the original, process name extracted as the final path component, process identifier converted from string to integer, command line arguments preserved, user domain extracted from the prefix of the user context string, and user name extracted from the suffix.

This transformation enables security analytics platforms to query for process creation events using standardized field names, filter by specific executables or command patterns, and correlate with user activity across different event types using consistent user identification fields.

### Network Connection Events

A Sysmon network connection event contains timestamp of connection establishment, event identifier value of three, hostname of the originating system, and data including the executable path of the process initiating the connection, process identifier, source IP address and port number, destination IP address and port number, protocol identifier such as TCP or UDP, and user context.

The ECS representation maps these fields to standardized network schema components including event category set to "network", event action set to "connection_established", network protocol field, source IP and port fields under the source namespace, destination IP and port fields under the destination namespace, process context fields, and parsed user identification fields.

This standardization enables network traffic analysis queries that correlate connection patterns with process behavior, identify lateral movement through unusual destination patterns, and attribute network activity to specific user accounts.

### File Creation Events

A Sysmon file creation event contains timestamp of file creation, event identifier value of eleven, hostname where creation occurred, and data including the executable that created the file, process identifier, target filename with full path, and user context.

The ECS mapping produces event category set to "file", event action set to "create", file path containing the complete directory and filename, file name extracted as the final path component, process context identifying the creating executable, and user identification fields.

This standardization supports file system monitoring use cases including detection of files created in suspicious locations, tracking of document creation by specific processes, and attribution of file system changes to user accounts.

### Registry Modification Events

A Sysmon registry event contains timestamp of modification, event identifier value of thirteen, hostname where modification occurred, and data including the modifying executable, process identifier, target registry object path such as HKCU\Software\Application\Settings\Key, details about the modification operation, and user context.

The ECS representation includes event category set to "registry", event action set to "modification", registry path field containing the target object, registry value field containing modification details, process context, and user identification fields.

This standardization enables detection of persistence mechanisms through registry modification patterns, identification of unauthorized configuration changes, and tracking of registry access by specific applications.

### DNS Query Events

A Sysmon DNS query event contains timestamp of query execution, event identifier value of twenty-two, hostname originating the query, and data including the querying process executable, process identifier, query name representing the domain being resolved, query results containing resolved IP addresses, query status indicating success or failure, and user context.

The ECS representation maps to event category set to "network" with action set to "dns_query", DNS question name field, DNS answers field containing resolved addresses, DNS response code field indicating query outcome, process context, and user identification fields.

This standardization supports detection of DNS-based command and control channels, identification of data exfiltration through DNS tunneling techniques, and analysis of domain resolution patterns associated with malicious infrastructure.

---

## Performance Characteristics and Tuning

### Backpressure Management

The bounded channel architecture with wait-based full mode provides deterministic backpressure behavior. When event collection rate exceeds processing capacity, the channel fills to capacity and the collector blocks on subsequent enqueue operations. This blocking propagates back to the event source, creating natural rate limiting without event loss.

This behavior contrasts with unbounded queue designs where memory consumption grows without limit under sustained overload, and drop-based bounded queue designs where events are silently discarded when capacity is exceeded. The wait-based approach prioritizes data completeness at the cost of potential source-level delays during extreme load conditions.

Capacity planning should consider expected peak event rates and acceptable latency windows. Higher channel capacity provides greater buffering against transient processing delays but increases memory footprint. Lower capacity reduces memory usage but may induce backpressure more frequently during normal operations.

### Batch Processing Optimization

Batch writing significantly reduces disk I/O overhead compared to per-event writing strategies. By accumulating multiple events in memory and performing a single write operation, the system amortizes file system call overhead across the entire batch. The batch size parameter controls the trade-off between write efficiency and memory consumption.

Larger batch sizes improve throughput by reducing the frequency of expensive I/O operations, but increase memory requirements for the staging buffer and extend the maximum latency between event occurrence and persistent storage. Smaller batch sizes reduce memory pressure and latency but increase I/O frequency and overhead.

The flush interval parameter provides a time-based upper bound on write latency, ensuring that events are persisted within a predictable timeframe even when batch size thresholds are not reached. This dual threshold approach balances throughput optimization with latency constraints.

### Worker Pool Scaling

The worker count configuration parameter enables adjustment of normalization concurrency based on available CPU resources and observed processing bottlenecks. In CPU-bound scenarios where transformation logic consumes significant processing time, increasing worker count can improve throughput by parallelizing the mapping operations across multiple cores.

However, excessive worker counts may introduce contention for shared resources such as the channel or batch writer staging buffers, reducing efficiency. Additionally, context switching overhead becomes significant when worker count substantially exceeds available CPU cores. Optimal worker count typically ranges from one to two times the number of logical processors, depending on the ratio of CPU-bound transformation logic to I/O-bound write operations.

Performance monitoring should track channel depth, events processed per second, and individual worker utilization to identify appropriate scaling parameters for specific deployment environments and workload characteristics.

### Resource Utilization

Memory consumption in the system derives primarily from three sources: queued events in the bounded channel, staging buffers in the batch writers, and worker thread stack allocations. Channel memory usage scales linearly with capacity and average event size, with typical Sysmon events consuming several hundred bytes including field data and object overhead.

Batch writer staging buffers accumulate events up to the configured batch size, with memory requirements proportional to batch size times average event size. Multiple concurrent batches may exist in memory simultaneously across raw and normalized writers.

Worker thread allocations include stack space and any intermediate data structures created during transformation. With typical stack sizes around one megabyte per thread and minimal heap allocations in the mapping logic, worker memory overhead remains modest even with elevated worker counts.

Disk I/O patterns exhibit bursty behavior aligned with flush intervals and batch size thresholds. Sequential write patterns to JSONL files provide favorable performance characteristics on most storage systems. Asynchronous I/O operations prevent thread blocking during writes, maintaining system responsiveness during disk operations.

---

## Operational Considerations

### Configuration Examples

A typical production configuration might specify a batch size of fifty events, balancing write efficiency with memory usage; a flush interval of one thousand milliseconds, ensuring sub-second latency for event persistence; a channel capacity of one thousand events, providing buffering for transient processing delays without excessive memory consumption; a worker count of four, matching a quad-core processor configuration; a raw file path pointing to a dedicated logging volume with appropriate permissions; and a normalized file path similarly configured for operational analytics consumption.

Development or testing configurations might reduce batch size and flush interval for more immediate feedback, decrease channel capacity to more quickly identify processing bottlenecks, and adjust file paths to local development directories.

### Deployment Modes

The collect-only deployment mode supports distributed architectures where event collection occurs on monitored endpoints while processing occurs on dedicated analysis infrastructure. Raw events are written to network-accessible storage or forwarded through other transport mechanisms for centralized processing.

The normalize-only deployment mode enables dedicated processing nodes that consume from centralized raw event repositories. This separation allows independent scaling of collection and processing tiers based on their respective resource requirements and bottlenecks.

The integrated both mode provides complete functionality in a single process, suitable for standalone deployments or smaller environments where architectural complexity of separate collection and processing tiers is unwarranted.

Command line invocation specifies the desired mode through flags, enabling flexible deployment without code modifications or separate build artifacts.

### Monitoring and Diagnostics

Operational monitoring should track several key metrics to ensure healthy system operation. Channel depth indicates the backlog of unprocessed events, with consistently high values suggesting insufficient processing capacity relative to collection rate. Events processed per second provides throughput measurements for capacity planning and performance validation.

Batch write frequency and size distributions reveal actual batching behavior under operational load, helping validate that configuration parameters achieve intended performance characteristics. Worker utilization metrics identify whether workers are CPU-bound in transformation logic or I/O-bound waiting for write operations.

Error rates for deserialization failures, mapping exceptions, and write errors indicate data quality issues or system problems requiring investigation. Graceful shutdown completion time measures how long the system requires to flush pending data during shutdown sequences.

Log output provides diagnostic information including configuration values at startup, periodic status updates during operation, error details for failed operations, and shutdown sequence progress. Structured logging formats enable integration with centralized log aggregation platforms for enterprise-scale deployments.

### Troubleshooting Common Issues

Task cancellation exceptions during shutdown indicate improper coordination of cancellation tokens and graceful shutdown sequences. Verify that the collector properly completes channel writing before cancellation propagates to consumers, and that consumers check cancellation tokens while continuing to process remaining channel contents.

Memory growth over time may indicate insufficient channel capacity causing event queuing upstream of the channel, or memory leaks in transformation or serialization logic. Monitor channel depth and batch writer memory usage to isolate the source of memory pressure.

Performance degradation under sustained load suggests inadequate worker count for the transformation workload, insufficient disk I/O bandwidth for write operations, or contention in the batch writer staging logic. Profiling tools can identify specific bottlenecks for targeted optimization.

Data loss during shutdown typically results from premature process termination before batch writers flush pending data. Ensure that shutdown signals properly propagate through the cancellation token and that the application waits for all consumer tasks to complete before final termination.

---

## Future Enhancement Opportunities

### File Rotation

Implementing file rotation when output files reach configured size or age thresholds would enable long-running deployments without manual intervention for log management. Rotation logic would close current output files, rename them with timestamp suffixes, and open new files for subsequent writes. Integration with log retention policies could automatically archive or delete rotated files based on organizational requirements.

### Advanced Telemetry

Enhanced telemetry integration with platforms such as Prometheus would provide standardized metrics exposition for operational dashboards and alerting. Metrics could include cumulative event counts by type, processing latency percentiles, error rates by category, and resource utilization measurements. Windows Event Tracing integration would enable deep performance analysis during troubleshooting.

### Automated Capacity Tuning

Adaptive configuration adjustment based on observed operational characteristics could optimize system parameters without manual intervention. The system could monitor channel depth and worker utilization patterns, automatically adjusting worker count within configured bounds to maintain target utilization levels. Similar adaptive strategies could optimize batch size and flush intervals based on observed latency and throughput distributions.

### Data Quality Validation

Enhanced validation of incoming Sysmon events against expected schemas and value ranges would identify data quality issues before they propagate into normalized output. Validation failures could trigger alerting for investigation of monitoring infrastructure problems or unexpected event format changes requiring mapping logic updates.

---

## Conclusion

The EDR Agent system provides a robust, scalable architecture for security telemetry normalization, leveraging modern concurrent programming patterns to achieve high throughput with deterministic behavior under load. The producer-consumer design with bounded channels ensures reliable operation without data loss, while batch processing strategies optimize resource utilization. Extensive configurability enables deployment-specific tuning without code modifications, supporting diverse operational requirements across development, testing, and production environments.