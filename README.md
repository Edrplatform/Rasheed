# OpenEDR 
[![OpenEDR](https://techtalk.comodo.com/wp-content/uploads/2020/09/logo_small.jpg)](https://openedr.com/)
[![Slack](https://img.shields.io/badge/slack-join-blue.svg)](https://openedr.com/register/) [![Email](https://img.shields.io/badge/email-join-blue.svg)](mailto:register@openedr.com)

[![OpenEDR - Getting Started](https://techtalk.comodo.com/wp-content/uploads/2022/10/Screenshot_3.jpg)](https://www.youtube.com/watch?v=lfo_fyinvYs "OpenEDR - Getting Started")    

We at OpenEDR believe in creating a cybersecurity platform with its source code openly available to the public,  where products and services can be provisioned and managed together. EDR is our starting point.
OpenEDR is a full-blown EDR capability. It is one of the most sophisticated, effective EDR code base in the world and with the community’s help, it will become even better.

OpenEDR is free and its source code is open to the public. OpenEDR allows you to analyze what’s happening across your entire environment at the base-security-event level. This granularity enables accurate root-causes analysis needed for faster and more effective remediation. Proven to be the best way to convey this type of information, process hierarchy tracking provides more than just data, they offer actionable knowledge. It collects all the details on endpoints, hashes, and base and advanced events. You get detailed file and device trajectory information and can navigate single events to uncover a larger issue that may be compromising your system.

OpenEDR’s security architecture simplifies *breach detection, protection, and visibility* by working for all threat vectors without requiring any other agent or solution. The agent records all telemetry information locally and then will send the data to locally hosted or cloud-hosted ElasticSearch deployments. Real-time visibility and continuous analysis are vital elements of the entire endpoint security concept. OpenEDR enables you to perform analysis into what's happening across your environment at base event level granularity. This allows accurate root cause analysis leading to better remediation of your compromises. Integrated Security Architecture of OpenEDR delivers Full Attack Vector Visibility including MITRE Framework. 

## Quick Start
The community response to OpenEDR has been absolutely amazing! Thank you. We had a lot of requests from people who want to deploy and use OpenEDR easily and quickly. We have a roadmap to achieve all these. However in the meanwhile, we have decided to use the Comodo Dragon Enterprise platform with OpenEDR to achieve that. By simply opening an account, you will be able to use OpenEDR. No custom installation, no log forwarding configuration, or worrying about storing telemetry data. All of that is handled by the Comodo Dragon Platform. This is only a short-term solution until all the easy-to-use packages for OpenEDR is finalized. In the meanwhile do take advantage of this by emailing quick-start@openedr.com to get you up and running!


## Components
The Open EDR consists of the following components:

* Runtime components
  * Core Library – the basic framework; 
  * Service – service application;
  * Process Monitor – components for per-process monitoring; 
    * Injected DLL – the library which is injected into different processes and hooks API calls;
    * Loader for Injected DLL – the driver component which loads injected DLL into each new process
    * Controller for Injected DLL – service component for interaction with Injected DLL;
  * System Monitor – the genetic container for different kernel-mode components;
  * File-system mini-filter – the kernel component that hooks I/O requests file system;
  * Low-level process monitoring component – monitors processes creation/deletion using system callbacks
  * Low-level registry monitoring component – monitors registry access using system callbacks
  * Self-protection provider – prevents EDR components and configuration from unauthorized changes
  * Network monitor – network filter for monitoring the network activity;
* Installer

Generic high-level interaction diagram for runtime components

![](https://techtalk.comodo.com/wp-content/uploads/2020/09/image.png)

For details, you can refer here: https://techtalk.comodo.com/2020/09/19/open-edr-components/

# Community
* Community Forums: https://community.openedr.com/
* Join Slack [![Slack](https://img.shields.io/badge/slack-join-blue.svg)](https://openedr.com/register/)
* Registration [![Email](https://img.shields.io/badge/email-join-blue.svg)](mailto:register@openedr.com)

# Roadmap
Please refer here for project roadmap : https://github.com/ComodoSecurity/openedr_roadmap/projects/1

## Getting Started

Please take a look at the following documents.

1. [Getting Started]
# Installation Instructions
OpenEDR is a single agent that can be installed on Windows endpoints. It generates extensible telemetry data for overall security-relevant events. It also uses file lookup, analysis, and verdict systems from Comodo, https://valkyrie.comodo.com/. You can also have your own account and free license there.

The telemetry data is stored locally on the endpoint itself. You can use any log streaming solution and analysis platform. Here we will present, how can you do remote streaming and analysis via open source tools like Elasticsearch ELK and Filebeat.

## OpenEDR:
OpenEDR project will release installer MSI’s signed by Comodo Security Solutions, The default installation folder is C:\Program Files\OpenEdr\EdrAgentV2

The agent outputs to C:\ProgramData\edrsvc\log\output_events by default, there you will see the EDR telemetry data where you should point this to Filebeat or other log streaming solutions you want. Please check following sections for details

## Docker and Docker Compose:
Docker is widely known virtualizon method for almost all environments you can use use docker for installing Elasticsearch and Log-stash to parse and more visibiltiy we used Docker to virtualize and make an easy setup process for monitoring our openedr agent. You can visit and look for more details and configure from https://github.com/docker


## Logstash:
Logstash is an open-source data ingestion tool that allows you to collect data from a variety of sources, transform it, and send it to your desired destination. With pre-built filters and support for over 200 plugins, Logstash allows users to easily ingest data regardless of the data source or type. We used Logstash to simplify our output to elasticsearch for more understandable logs and easily accessible by everyone who uses openedr and this example. You may visit and configure your own system as well https://github.com/elastic/logstash

## Elasticsearch:
There are multiple options to run Elasticsearch, you can either install and run it on your own machine, on your data center, or use Elasticsearch service on public cloud providers like AWS and GCP. If you want to run Elasticsearch by yourself. You can refer to here for installation instructions on various platforms https://www.elastic.co/guide/en/elasticsearch/reference/current/install-elasticsearch.html

Another option is using Docker, this will also enable a quick start for PoC and later can be extended to be production environment as well. You can access the guide for this setup here: https://www.elastic.co/guide/en/elasticsearch/reference/7.10/docker.html
## Filebeat:
Filebeat is a very good option to transfer OpenEDR outputs to Elasticsearch, you need to install Filebeat on each system you want to monitor. Overall instructions for it can be found here: https://www.elastic.co/guide/en/beats/filebeat/current/filebeat-installation-configuration.html 

We don’t have OpenEDR Filebeat modules yet so you need to configure a custom input option for filebeat https://www.elastic.co/guide/en/beats/filebeat/current/configuration-filebeat-options.html

## Example on the go
We used these given technologies for an even simpler solution for example and you can use these methods and configure them for your own need even use different technologies depending on your needs.

> Install docker on the device
> Clone pre-prepared ELK package
> Run docker-compose.yaml with docker-compose
> Install openedr and filebeat on the client device
> Configure filebeat to your elastic search and logstash

 All configure and installation documents are can be found in Getting Started sections
 

[Build Instructions](BuildInstructions.md)

[Docker Installation](DockerInstallation.md)

[Setting up Elasticsearch Kibana and Logstash](SettingELK.md)

[Setting up Openedr and File beat](SettingFileBeat.md)

[Editing Alerting Policies](EditingAlertingPolicies.md)

[Setting Up Kibana](SettingKibana.md)


2. [Build Instructions](getting-started/BuildInstructions.md)

# Build Instructions
You should have Microsoft Visual Studio 2019 to build the code
* Open Microsoft Visual Studio 2019 as Administrator under  /openedr/edrav2/build/vs2019/
* Open the edrav2.sln solutions file within the folder
* Build Release on edrav2.sln on Visual Studio
* Open Microsoft Visual Studio 2019 as Administrator under  /openedr/edrav2/iprj/installations/build/vs2019/
* Open installation.wixproj
* Compile installer.
```
As a default You should have these libraries in you Visual Studio 2019 but these are needed for build
## Libraries Used:
* AWS SDK AWS SDK for C++ : (https://aws.amazon.com/sdk-for-cpp/)
* Boost C++ Libraries (http://www.boost.org/)
* c-ares: asynchronous resolver library (https://github.com/c-ares/c-ares)
* Catch2: a unit testing framework for C++ (https://github.com/catchorg/Catch2)
* Clare : Command Line parcer for C++ (https://github.com/catchorg/Clara)
* Cli: cross-platform header-only C++14 library for interactive command line interfaces (https://cli.github.com/) 
* Crashpad: crash-reporting system (https://chromium.googlesource.com/crashpad/crashpad/+/master/README.md)
* Curl: command-line tool for transferring data specified with URL syntax (https://curl.haxx.se/)
* Detours: for monitoring and instrumenting API calls on Windows. (https://github.com/microsoft/Detours)
* Google APIs: Google APIs (https://github.com/googleapis/googleapis)
* JsonCpp: C++ library that allows manipulating JSON values (https://github.com/open-source-parsers/jsoncpp)
* libjson-rpc-cpp: cross platform JSON-RPC (remote procedure call) support for C++  (https://github.com/cinemast/libjson-rpc-cpp)
* libmicrohttpd : C library that provides a compact API and implementation of an HTTP 1.1 web server (https://www.gnu.org/software/libmicrohttpd/)
* log4cplus: C++17 logging API (https://github.com/log4cplus/log4cplus)
* nlohmann JSON: JSON library for C++: (https://github.com/nlohmann/json)
* OpenSSL Toolkit (http://www.openssl.org/)
* Tiny AES in C: implementation of the AES ECB, CTR and CBC encryption algorithms written in C. (https://github.com/kokke/tiny-AES-c)
* Uri: C++ Network URI (http://www.boost.org/)
* UTF8-CPP: UTF-8 with C++ (http://utfcpp.sourceforge.net/)
* xxhash_cpp: xxHash library to C++17. (https://cyan4973.github.io/xxHash/)
* Zlib: Compression Libraries (https://zlib.net/)


3. [Docker Installation](getting-started/DockerInstallation.md)
### Docker installation

 * For Linux distros visit and follow instructions at https://docs.docker.com/desktop/install/linux-install/
    * in our example we used Ubuntu-Server 22.04 LTS you may go with that since gnome-terminal which is needed by docker.
 simply run with the order
 ```console
  apt-get update
  apt-get upgrade
  apt install docker
 ```
 use sudo if you don't have root access

 

4. [Setting up Elasticsearch Kibana and Logstash](getting-started/SettingELK.md)

### Setting up Elasticsearch Kibana and Logstash

 * You can get the pre-configured package at https://github.com/deviantony/docker-elk
    also, you can configure your system defaults also work but less securely please check  https://github.com/deviantony/docker-elk/blob/main/README.md for further information on configuration details

 ![git clone](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/git-clone-elk.png)

   Clone or download the repository
   Open Terminal inside repo and run
```console
$ sudo docker-compose up -d
```
   -d is for running at the background and if permissions are asked please re-run with sudo privileges

 ![Docker compose up](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/docker-compose-allup.png)

   You should be able to see docker containers.
    
check with
```console
sudo docker ps
```
![Docker ps](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/docker-ps-list.png)

   keep in mind kibana is the log ui in this setup  we gonna use kibanas port later on
    by now your monitoring tools are up and running
    Reminder default password and user would be 
     Username : elsatic
     Password : changeme


5. [Setting up Openedr and File beat](getting-started/SettingFileBeat.md)

### Setting up Openedr and File beat
 * openedr is a simple msi installer most of the current windows machines are capable you can use the installer and its own instructions.
 * Filebeat is a log analysis tool you can download the installer and follow up on its instructinos https://www.elastic.co/downloads/beats/filebeat

    First, you need to enable logstash within Filebeat to do that
    >Open Powershell as administrator
    >Change directory to within Powershell "C:\Program Files\Elastic\Beats\8.4.1\filebeat"
    > run this command
```console
.\filebeat.exe modules enable logstash --path.config"C:\ProgramData\Elastic\Beats\filebeat"
```
![Filebeat module](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/filebeat-enable-module-logstash.png)

This command will enable logstash feature and choose your configuration path

 * For filebeat configuration first go to C:\ProgramData\Elastic\Beats\filebeat
   You can check out the filebeat.example.yaml and edit that for your needs.
   and now we need to create a file as "filebeat.yaml" inside the directory.
    >Tip you can copy filebeat.example.yaml and just edit from there
   Your filebeat.yaml needs these configurations activated and edited for your IP addresses and usage
```console
   
filebeat.inputs:

-type: filestream

  id: edr

  enabled: true
     paths:

    -C:\ProgramData\edrsvc\log\*



filebeat.config.modules:
  
  path: ${path.config}/modules.d/*.yml

  reload.enabled: false
  
setup.template.settings:
  index.number_of_shards: 1

setup.kibana:

    output.logstash:
     hosts: ["Your docker adress:5044"]
```
![filebeat config](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/filebeatinputs-filebeatyaml.png)
![filebeat config](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/filebeatmodules-filebeatyaml.png)
![filebeat config](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/logstashconfig-filebeatyml.png)

* Now we have to configure activated logstash on filebeat go to  C:\ProgramData\Elastic\Beats\filebeat\modules.d\
    You can check out logstash.yaml and edit accourding to your needs and configuration.
    Edit logstash.yaml as accordingly

```console
module: logstash

  log:
    enabled: true


    var.paths:
     - C:\ProgramData\edrsvc\log\*

  slowlog:
    enabled: false
```
 ![Logstash Config](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/logstash-yaml.png)

 as for the final step to run filebeat with these configurations please restart the filebeat service from your services.msc
 or run this command from your Powershell as an administrator

![Services msc](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/services-filebeat-restart.png)

 ```console
    Restart-Service -Force filebeat
 ```
 ![Powershell Services](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/filebeat-service-restart.png)


6. [Editing Alerting Policies](getting-started/EditingAlertingPolicies.md)

## Editing Alerting Policies 
The agent uses network driver, file driver, and DLL injection to capture events that occur on the endpoint. It enriches the event data with various information, then filters these events according to the policy rules and sends them to the server. 

You can customize your policy with your own policy. Within the installation folder which is "C:\Program Files\Comodo\EdrAgentV2" policy file called   "evm.local.src"

For Comodo suggested rules please check the rule repo https://github.com/ComodoSecurity/OpenEDRRules

You can edit this file with any text editor and customize your own policy accordingly with the given information below.
```console
{
    "Lists": {
        "List1": ["string1", "string2", ...],
        "List2": ["string1", "string2", ...],
        ...
    },
    "Events": {
        "event_code": [<AdaptiveEvent1>, <AdaptiveEvent2>, ...],  --> The order of the adaptive events in the list is important (see Adaptive Event Ordering).
        "event_code": [<AdaptiveEvent1>, <AdaptiveEvent2>, ...],
        ...
    }
}
```

### Adaptive Event
```console
{
    "BaseEventType": 3
    "EventType": null|GUID,
    "Condition": <Condition>
}
```

### Condition
```console
{
    "Field": "parentVerdict",
    "Operator": "!Equal",
    "Value": 1
}
```
Boolean operators are supported. The following also is a condition:
```console
Equal",
    "Value": 1
}
```
Boolean operators are supported. The following is also a condition:
```console
{
    "BooleanOperator": "Or",
    "Conditions": [
        {
            "Field": "parentVerdict",
            "Operator": "!Equal",
            "Value": 1
        },
        {
            "Field": "parentProcessPath",
            "Operator": "MatchInList",
            "Value": "RegWhiteList"
        }
    ]
}
```
Nesting with Boolean operators are supported. The following is also a condition:
```console
{
    "BooleanOperator": "And",
    "Conditions": [
        {
            "Field": "parentProcessPath",
            "Operator": "!Match",
            "Value": "*\\explorer.exe"
        },
        {
            "BooleanOperator": "Or",
            "Conditions": [
                {
                    "Field": "path",
                    "Operator": "Match",
                    "Value": "*\\powershell.exe"
                },
                {
                    "Field": "path",
                    "Operator": "Match",
                    "Value": "*\\cmd.exe"
                }
            ]
        }
    ]
}

```
The BooleanOperator field can be one of the following:

>and

>or

The Operator field can be one of the following:
| Operator | Value Type | Output |
| --- | --- | --- |
| `Equal` | Number, String, Boolean, null | true if the field and the value are equal|
| `Match`| String (wildcard pattern)| true if the field matches the pattern (environment variables in the pattern must be expanded first)|
| `MatchInList` | String (list name) | true if the field matches one of the patterns in the list (environment variables in patterns must be expanded first)|


!Equal, !Match and !MatchInList must also be supported.

### Adaptive Event Ordering
When an event is captured, the conditions in the adaptive events are checked sequentially. When a matching condition is found, the BaseEventType and EventType in that adaptive event are added to the event data and the event is sent. Other conditions are not checked. No logging will be done if no advanced event conditions match. If the Condition field is not provided, it is assumed that the condition matches.


7. [Setting Up Kibana](getting-started/SettingKibana.md)

### Setting Up Kibana

 Kibana is Uİ based Monitoring system. The logstash and elasticsearch environment can handle most of the logging systems such as openedr

>First set up logstash in kibana
 ![Welcome message](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui1.png)

 >Within browse integrations you can find logstash right away
 ![finding logstsash](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui2.png)

 > We have already managed Filebeat configurations so you may just scroll down and go to logstash logs
 ![Logstash logs](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui3.png)

 > Create data View to see logs and outputs
 ![Data view](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui4.png)

 > Check Log stream names and include index patterns like or just as coming indexes
 ![index pattern](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui5.png)

 > Create a new Dashboard Configure for your requirements with coming filed abd graphs and example of time and log count
 ![Dashboard config](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui6.png)

  You can create more dash boards as you like
    
 > You can find your metrics with in dashboard 
 ![Metrics](https://github.com/ComodoSecurity/openedr/blob/main/docs/screenshots/elastic%20ui7.png)

 
 # Releases
https://github.com/ComodoSecurity/openedr/releases/tag/release-2.5.1
# Screenshots
How OpenEDR integration with a platform looks like and also a showcase for openedr capabilities

Detection / Alerting
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_1.jpg)](https://enterprise.comodo.com/dragon/)

Event Details
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_2.jpg)](https://enterprise.comodo.com/dragon/)

Dashboard
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_3.jpg)](https://enterprise.comodo.com/dragon/)

Process Timeline
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_4.jpg)](https://enterprise.comodo.com/dragon/)

Process Treeview
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_5.jpg)](https://enterprise.comodo.com/dragon/)


Event Search
[![OpenEDR](https://github.com/Edrplatform/Rasheed/blob/main/screenshots/Screenshot_6.jpg)](https://enterprise.comodo.com/dragon/)



# Contribution
We need your contribution more than anything else. There are numerous ways to help this project.

## Use it and provide feedback
If you use it, let us know what works and what does not work. Open an issue about it labeling bug, new feature request, open a issue and label it as enchancement

## Questions 
Open an issue label it question, also please help us to answer the question as well 

## Tell us about false positives
Improve our rules, correct it if necessary, please use similiar as bug/feature request, 

## Work on open issues
We love pull requests from everyone. Either bug/feature requests, Take a task Fork, clone the repo and submit a pull request.
Please do PR to the ‘develop’ branch only. 
The 'main' branch stores flat history of all released versions. A tip of the 'main' branch points to the current version. Any direct commits to the main branch by developers are prohibited.

## Testing
We need extensive testing since we might introduce some bugs to make it open source in hurry, please test it in various platforms and report your findings as issues

## Documentation
We are lack of effective documentation right now, please provide any kind of knowledgebase, case studies, analysis results, etc.

## Spread the word
Last but not least, the more people use OpenEDR, the better we will be, please help us by sharing it via social media or any other channels


# LICENSE

COMODO AVAILABLE SOURCE LICENSE (CASL) AGREEMENT

BY DOWNLOADING, INSTALLING, ACCESSING OR USING THE SOFTWARE, YOU ACKNOWLEDGE   THAT YOU HAVE READ THIS AGREEMENT, THAT YOU UNDERSTAND IT, AND THAT YOU AGREE TO BE BOUND BY ITS TERMS. IF YOU DO NOT AGREE TO THE TERMS HEREIN, DO NOT USE THE SOFTWARE.

THIS AGREEMENT CONTAINS A BINDING ARBITRATION PROVISION THAT REQUIRES THE RESOLUTION OF DISPUTES ON AN INDIVIDUAL BASIS, LIMITS YOUR ABILITY TO SEEK RELIEF IN A COURT OF LAW, AND WAIVES YOUR RIGHT TO PARTICIPATE IN CLASS ACTIONS, CLASS ARBITRATIONS, OR A JURY TRIAL FOR CERTAIN DISPUTES.

If you are receiving the software on behalf of a legal entity, you represent and warrant that you have
the actual authority to agree to the terms and conditions of this agreement on behalf of such entity. 
 
This Agreement is between you (“You”) and Comodo Security Solutions, Inc., which has its principal place of business at 1255 Broad Street, Suite 100, Clifton, New Jersey 07013, and shall be referred to herein as “Comodo” or “Licensor.”

In exchange for your use of the Software, you agree as follows:
# 1.	 License

1.1	Subject to the terms and conditions of this Section 1, Licensor hereby grants to You a non-exclusive, royalty-free, worldwide, non-transferable license during the term of this Agreement to:

(a)	distribute or make available the Software or your modifications under the terms of this Agreement without charge, and only as part of your free application, so long as you include the following notice on any copy you distribute: “This software is subject to the terms of the Comodo Available Source License Agreement.”    

(b)	use the Software or your modifications without charge and only as part of your free application, but not in connection with any product that is distributed or otherwise made available by any third party.  

(c)	modify the Software, provided that all modifications remain subject to the terms of this License.

(d)	reproduce the Software as necessary for the above. 

(e)	the above terms are conditional on You not charging any fees for the Software, modifications or any application which uses the Software or a modification thereof.  Any attempt at charging or collecting fees from the Software, modifications, or applications automatically terminates the License with all rights reverting back to Comodo.

1.2	Sublicensing:  You may sublicense, for no charge only, the right to use the Software fully embedded in your free application as distributed by you in accordance with Section 1.1(a), pursuant to a written license that disclaims all warranties and liabilities on behalf of Licensor.

1.3	Notices:  On all copies of the Software that you make, you must retain all copyright or other proprietary notices.

1.4	Software for Services: MSSPs (Managed Security Services Providers) or SOCs (Security Operation Centers) can use the Software, with or without modifying, and can create service offerings around the Software. MSSPs or SOCs may charge for these services. The above licensing grant in Sections 1.1 through 1.3 does not prohibit this use case.

# 2.	Ownership

2.1	No Ownership Rights.  The Software is being licensed, not sold.  Comodo retains all ownership rights in and to all Software, including any intellectual property rights therein.  

2.2	Copyright.  The Software contains material that is protected by United States and foreign intellectual property laws, including copyright, trade secret, and patent law.  All rights not granted to you herein are expressly reserved by Comodo. You may not remove any copyright or other proprietary notice from the Software.

2.3	Updates.  Comodo is under no obligation to provide update, support or patches for the Software.  

# 3.	Termination

3.1	Term.  This agreement is effective until terminated by you or by Comodo. Any breach any of the terms, obligations, and conditions herein by You under this Agreement, terminates this Agreement automatically and the licenses granted herein will terminate automatically. 

Termination by You.  You may terminate this agreement at any time by removing all copies of the software in your possession or under your control. 

 
# 4.	Disclaimers and Limitation of Liability
 
4.1	Guarantee Disclaimer; Assumption of Risk.  EXCEPT AS SPECIFICALLY STATED OTHERWISE IN THIS AGREEMENT, COMODO EXPRESSLY DISCLAIMS ALL IMPLIED AND EXPRESS WARRANTIES IN THE SOFTWARE.  THIS DISCLAIMER INCLUDES ALL WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AND NON-INFRINGEMENT AND IS EFFECTIVE TO THE MAXIMUM EXTENT ALLOWED BY LAW.  COMODO DOES NOT GUARANTEE THAT 1) THE SOFTWARE WILL MEET YOUR REQUIREMENTS OR EXPECTATIONS OR 2) THAT ACCESS TO THE SOFTWARE WILL BE UNINTERRUPTED, TIMELY, SECURE, OR ERROR-FREE.  The SOFTWARE IS PROVIDED “AS IS."  

COMODO HAS NO LIABILITY OR RESPONSIBILITY FOR ANY PRE-EXISTING CONDITION ON YOUR COMPUTERS, DEVICES OR SYSTEMS, INCLUDING FAILURE TO CURE, DETECT OR REMEDY THE CONDITION AFTER INSTALLATION AND USE OF THE SOFTWARE.

4.2      Damage Limitation. TO THE MAXIMUM EXTENT ALLOWABLE UNDER LAW, LICENSOR WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND, INCLUDING BUT NOT LIMITED TO LOST PROFITS, DATA OR ANY CONSEQUENTIAL, SPECIAL, INCIDENTAL, INDIRECT, OR DIRECT DAMAGES, ARISING OUT OF OR RELATING TO THIS AGREEMENT.

# 5.	General/Miscellaneous

5.1	Entire Agreement.  This agreement, along with any attached schedules and any documents referred to herein, is the entire agreement between the parties with respect to the subject matter, superseding all other agreements that may exist with respect to the subject matter.  Section headings are for reference and convenience only and are not part of the interpretation of the agreement.

5.2		Waiver.  A party’s failure to enforce a provision of this agreement does not waive the party’s right to enforce the same provision later or right to enforce any other provision of this agreement.  To be effective, all waivers must be both in writing and signed by the party benefiting from the waived provision.

5.3	Arbitration and Governing Law.  ARBITRATION MEANS THAT YOU WAIVE YOUR RIGHT TO A JUDGE OR JURY TRIAL IN A COURT PROCEEDING AND YOUR GROUNDS FOR APPEAL ARE LIMITED

You agree that any dispute, claim or controversy arising out of this agreement shall be determined by binding arbitration. Before you may begin arbitration with respect to a dispute involving any aspect of this Agreement, you shall notify Comodo and any other party to the dispute for the purpose of seeking dispute resolution. The notice to Comodo should be addressed to 1255 Broad Street, Clifton, New Jersey 07013.

If the dispute is not resolved within sixty (60) days after the initial notice, then a
party may proceed in accordance with the following: Any unresolved dispute arising under the terms of this Agreement shall be decided by arbitration conducted through the services of the Commercial Arbitration Rules of the American Arbitration Association (hereinafter referred to as the “AAA”). Notice of demand for an arbitration hearing shall be in writing and properly served upon the parties to this Agreement. Arbitration hearings shall be held in the state of New Jersey at a location mutually agreeable to the parties.

The laws of the state of New Jersey govern the interpretation, construction and enforcement of this agreement and all proceedings arising out of it without regard to any conflicts of laws principles.  Both parties agree to the exclusive venue and jurisdiction of state or U.S. federal courts located in New Jersey.

The United Nations Convention on Contracts for the International Sale of Goods and the Uniform Computer Information Transaction Act shall not apply to this agreement and are specifically excluded.

Any proceedings to resolve or litigate any dispute in any forum will be conducted solely on an individual basis. Neither you nor Comodo will seek to have any dispute heard as a class action, private attorney general action, or in any other proceeding in which either party acts or proposes to act in a representative capacity. No arbitration or proceeding will be combined with another without the prior written consent of all parties to all affected arbitration or proceedings.

5.4	Assignment. You may not assign any of your rights or obligations under this agreement, whether by merger, consolidation, operation of law, or any other manner, without the prior written consent of Comodo.  For purposes of this section only, a change in control is deemed an assignment.  Any transfer without consent is void.  To the extent allowed by law, Comodo may assign its rights and obligations without your consent.

5.5	Severability.  In the event that any provision is held invalid, unconscionable, or unenforceable in any manner, the licenses under this Agreement automatically terminate.

 
# SCHEDULE A
Included Software and Licenses

The following third party or open source software may be included and is provided under other licenses and/or has source available from other locations.

For Open EDR:

The build produces one set of binaries, which all fall under the CASL Agreement. The binaries also contain the following source codes that are licensed separately, and all public releases of EDR by Comodo Security Solutions, Inc. contain the following licenses acquired by Comodo Security Solutions, Inc.
•	https://netfiltersdk.com/license.html
•	http://help.madshi.net/License.htm
Therefore, the following source code contained with or used in Open EDR are specifically excluded from this License:
1.	Netfilter SDK : https://netfiltersdk.com/
2.	MadCodeHook: http://madshi.net/
Please contact the respective software owners directly for licensing information.

