using Dapr.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class OuputBindingSample : Example
    {
        private static readonly string bindingName = "bindingeventdemo";

        // Allow ouput binding operations: create, get, delete, list
        private static readonly string operation = "create";

        public override string DisplayName => "Using de Output Binding";

        public static string Data = @"AutoNavi is a leading provider of digital map and navigation services in China with over 100 million daily active users. AutoNavi started it’s Serverless/FaaS (Function as a service) project on April 2020 and after just one year, our solution already exceeds 100,000 queries per second (QPS). In this blog I will share how we at AutoNavi use Dapr to implement our serverless solution.

Below are a few of the business use cases that our solution addresses:

Weather in a long route - When the route distance exceeds a threshold, for example 100kms, weather information is provided along the route.
Search along route - Search for vehicle services such as gas station, electric vehicle charge-station etc. along the route.
Route tips - Show driver relevant route information such as warnings that large vehicles are ahead, narrow roads etc.
Scenic spots information - Show ticket prices, opening hours, contact information and brief descriptions for scenic spots and points of interest.
Why we chose Dapr
While building the new service, we encountered several challenges including two that we felt Dapr could help us address: Connecting existing backend services using a lightweight solution and our requirement for a runtime which supports multiple languages.

Connecting existing backend services
An important requirement we have is that our FaaS must have the ability to invoke existing backend micro-services which were developed on top of our RPC framework. A direct approach would be to use our RPC framework SDKs in our FaaS applications, but another requirement we have is keeping the FaaS runtime lightweight as to meet our fast start-up and scaling needs. We also wanted to avoid having the application become bloated due to a large number of SDKs that were integrated into the code base.

This is where Dapr’s lightweight footprint was very helpful. In addition, leveraging Dapr’s APIs helped us avoid using any SDK libraries in our code.

Multi-language runtime
At AutoNavi, we mainly use C++ and Node.js on the client side.Base navigation functions such as mapping display, route display, sign guidance, spoken guidance etc.need to run both on mobile and vehicle embedded navigation systems.These functions are written in C++ to leverage its strengths as a cross platform language. Other functions such as recommended points of interest after route completion are written in Node.js.In addition, on the server side, Java, Go and C++ are used.

We had designed a FaaS runtime component in our serverless solution so developers just need to write function code which will be downloaded, loaded to the runtime and finally run inside our FaaS runtime.To achieve this, we have developed different FaaS runtimes for each language such as C++, Go, Rust etc. Functions in each language need to connect to backend services or to infrastructure services such as Redis, MySQL, MQ and so on, so we needed a multi-language solution to help us achieve this if we wanted to avoid using class libraries for each of the different languages.

In the past our solution to this challenge was using a RSocket broker: The FaaS runtime uses a lightweight multi-language RSocket SDK to connect to the RSocket broker, the RSocket broker in turn forwards the request to our RPC framework proxy to invoke the backend micro-services and forwards the response back to the FaaS runtime.This indeed solved both challenges but introduced a new challenge - the RSocket broker and the RPC framework proxy are centralized and that breaks the decentralized architecture we aimed to have for our serverless system.

As we looked into using Dapr, we found that Dapr offers an optimal alternative solution.We use the Dapr sidecar to support multiple languages and keeping applications lightweight, replacing the need for client SDKs.Meanwhile, the sidecar pattern keeps our architecture decentralized without the need for a centralized broker like RSocket.

How Dapr is used in the FaaS runtime today


In our Dapr sidecar, we have developed our custom components to support our RPC framework and other infrastructure such as our own KV-Store, config server. The multi-language (C++/Node.js/Go/Java) FaaS runtime uses Dapr SDKs to make requests to the Dapr sidecar via gRPC and the Dapr sidecars make requests to our backend services or to infrastructures such as Redis, MySQL, MQ.and sends a callback to the FaaS runtime when a response is returned.

In this new serverless solution, the Dapr sidecar is injected automatically by our Kubernetes service when a new FaaS runtime pod created.We have integrated this in our CI/CD pipelines, the user function code and user configurations are the same on different environments because running in a dev environment and production is consistent thanks to Dapr APIs.

In practice we are still using Dapr in an experimental way.Currently RSocket broker serves as a fallback in case of failures with Dapr.We feel that having a fallback is always a best practice when adopting a new technology.

We are now working to verify Dapr in various scenarios by implementing Dapr in several business applications.After the verification, we see more and more parts of our solution migrating to use Dapr.Finally, we plan to remove the RSocket broker and fully rely on Dapr for our needs.

Summary
The solution above has now been running in AutoNavi’s production environment for over a month without any issues - the experiment is going well. By using Dapr, we solve the problems of invoking existing backend services in a lightweight model and supporting multiple languages in our serverless runtime without breaking the decentralized architecture.Dapr is really a perfect solution for invoking backend services in our multi-language serverless runtime.
传统部署时代：

早期，各个组织机构在物理服务器上运行应用程序。无法为物理服务器中的应用程序定义资源边界，这会导致资源分配问题。 例如，如果在物理服务器上运行多个应用程序，则可能会出现一个应用程序占用大部分资源的情况， 结果可能导致其他应用程序的性能下降。 一种解决方案是在不同的物理服务器上运行每个应用程序，但是由于资源利用不足而无法扩展， 并且维护许多物理服务器的成本很高。

虚拟化部署时代：

作为解决方案，引入了虚拟化。虚拟化技术允许你在单个物理服务器的 CPU 上运行多个虚拟机（VM）。 虚拟化允许应用程序在 VM 之间隔离，并提供一定程度的安全，因为一个应用程序的信息 不能被另一应用程序随意访问。

虚拟化技术能够更好地利用物理服务器上的资源，并且因为可轻松地添加或更新应用程序 而可以实现更好的可伸缩性，降低硬件成本等等。

每个 VM 是一台完整的计算机，在虚拟化硬件之上运行所有组件，包括其自己的操作系统。

容器部署时代：

容器类似于 VM，但是它们具有被放宽的隔离属性，可以在应用程序之间共享操作系统（OS）。 因此，容器被认为是轻量级的。容器与 VM 类似，具有自己的文件系统、CPU、内存、进程空间等。 由于它们与基础架构分离，因此可以跨云和 OS 发行版本进行移植。

容器因具有许多优势而变得流行起来。下面列出的是容器的一些好处：

敏捷应用程序的创建和部署：与使用 VM 镜像相比，提高了容器镜像创建的简便性和效率。
持续开发、集成和部署：通过快速简单的回滚（由于镜像不可变性），支持可靠且频繁的 容器镜像构建和部署。
关注开发与运维的分离：在构建/发布时而不是在部署时创建应用程序容器镜像， 从而将应用程序与基础架构分离。
可观察性：不仅可以显示操作系统级别的信息和指标，还可以显示应用程序的运行状况和其他指标信号。
跨开发、测试和生产的环境一致性：在便携式计算机上与在云中相同地运行。
跨云和操作系统发行版本的可移植性：可在 Ubuntu、RHEL、CoreOS、本地、 Google Kubernetes Engine 和其他任何地方运行。
以应用程序为中心的管理：提高抽象级别，从在虚拟硬件上运行 OS 到使用逻辑资源在 OS 上运行应用程序。
松散耦合、分布式、弹性、解放的微服务：应用程序被分解成较小的独立部分， 并且可以动态部署和管理 - 而不是在一台大型单机上整体运行。
资源隔离：可预测的应用程序性能。
资源利用：高效率和高密度。
为什么需要 Kubernetes，它能做什么?
容器是打包和运行应用程序的好方式。在生产环境中，你需要管理运行应用程序的容器，并确保不会停机。 例如，如果一个容器发生故障，则需要启动另一个容器。如果系统处理此行为，会不会更容易？

这就是 Kubernetes 来解决这些问题的方法！ Kubernetes 为你提供了一个可弹性运行分布式系统的框架。 Kubernetes 会满足你的扩展要求、故障转移、部署模式等。 例如，Kubernetes 可以轻松管理系统的 Canary 部署。

Kubernetes 为你提供：

服务发现和负载均衡

Kubernetes 可以使用 DNS 名称或自己的 IP 地址公开容器，如果进入容器的流量很大， Kubernetes 可以负载均衡并分配网络流量，从而使部署稳定。

存储编排

Kubernetes 允许你自动挂载你选择的存储系统，例如本地存储、公共云提供商等。

自动部署和回滚

你可以使用 Kubernetes 描述已部署容器的所需状态，它可以以受控的速率将实际状态 更改为期望状态。例如，你可以自动化 Kubernetes 来为你的部署创建新容器， 删除现有容器并将它们的所有资源用于新容器。

自动完成装箱计算

Kubernetes 允许你指定每个容器所需 CPU 和内存（RAM）。 当容器指定了资源请求时，Kubernetes 可以做出更好的决策来管理容器的资源。

自我修复

Kubernetes 重新启动失败的容器、替换容器、杀死不响应用户定义的 运行状况检查的容器，并且在准备好服务之前不将其通告给客户端。

密钥与配置管理

Kubernetes 允许你存储和管理敏感信息，例如密码、OAuth 令牌和 ssh 密钥。 你可以在不重建容器镜像的情况下部署和更新密钥和应用程序配置，也无需在堆栈配置中暴露密钥。

Kubernetes 不是什么
Kubernetes 不是传统的、包罗万象的 PaaS（平台即服务）系统。 由于 Kubernetes 在容器级别而不是在硬件级别运行，它提供了 PaaS 产品共有的一些普遍适用的功能， 例如部署、扩展、负载均衡、日志记录和监视。 但是，Kubernetes 不是单体系统，默认解决方案都是可选和可插拔的。 Kubernetes 提供了构建开发人员平台的基础，但是在重要的地方保留了用户的选择和灵活性。

Kubernetes：

不限制支持的应用程序类型。 Kubernetes 旨在支持极其多种多样的工作负载，包括无状态、有状态和数据处理工作负载。 如果应用程序可以在容器中运行，那么它应该可以在 Kubernetes 上很好地运行。
不部署源代码，也不构建你的应用程序。 持续集成(CI)、交付和部署（CI/CD）工作流取决于组织的文化和偏好以及技术要求。
不提供应用程序级别的服务作为内置服务，例如中间件（例如，消息中间件）、 数据处理框架（例如，Spark）、数据库（例如，mysql）、缓存、集群存储系统 （例如，Ceph）。这样的组件可以在 Kubernetes 上运行，并且/或者可以由运行在 Kubernetes 上的应用程序通过可移植机制（例如， 开放服务代理）来访问。
不要求日志记录、监视或警报解决方案。 它提供了一些集成作为概念证明，并提供了收集和导出指标的机制。
不提供或不要求配置语言/系统（例如 jsonnet），它提供了声明性 API， 该声明性 API 可以由任意形式的声明性规范所构成。
不提供也不采用任何全面的机器配置、维护、管理或自我修复系统。
此外，Kubernetes 不仅仅是一个编排系统，实际上它消除了编排的需要。 编排的技术定义是执行已定义的工作流程：首先执行 A，然后执行 B，再执行 C。 相比之下，Kubernetes 包含一组独立的、可组合的控制过程， 这些过程连续地将当前状态驱动到所提供的所需状态。 如何从 A 到 C 的方式无关紧要，也不需要集中控制，这使得系统更易于使用 且功能更强大、系统更健壮、更为弹性和可扩展。

控制平面组件（Control Plane Components） 
控制平面的组件对集群做出全局决策(比如调度)，以及检测和响应集群事件（例如，当不满足部署的 replicas 字段时，启动新的 pod）。

控制平面组件可以在集群中的任何节点上运行。 然而，为了简单起见，设置脚本通常会在同一个计算机上启动所有控制平面组件， 并且不会在此计算机上运行用户容器。 请参阅使用 kubeadm 构建高可用性集群 中关于多 VM 控制平面设置的示例。

kube-apiserver
API 服务器是 Kubernetes 控制面的组件， 该组件公开了 Kubernetes API。 API 服务器是 Kubernetes 控制面的前端。

Kubernetes API 服务器的主要实现是 kube-apiserver。 kube-apiserver 设计上考虑了水平伸缩，也就是说，它可通过部署多个实例进行伸缩。 你可以运行 kube-apiserver 的多个实例，并在这些实例之间平衡流量。

etcd
etcd 是兼具一致性和高可用性的键值数据库，可以作为保存 Kubernetes 所有集群数据的后台数据库。

您的 Kubernetes 集群的 etcd 数据库通常需要有个备份计划。

要了解 etcd 更深层次的信息，请参考 etcd 文档。

kube-scheduler
控制平面组件，负责监视新创建的、未指定运行节点（node）的 Pods，选择节点让 Pod 在上面运行。

调度决策考虑的因素包括单个 Pod 和 Pod 集合的资源需求、硬件/软件/策略约束、亲和性和反亲和性规范、数据位置、工作负载间的干扰和最后时限。

kube-controller-manager
运行控制器进程的控制平面组件。

从逻辑上讲，每个控制器都是一个单独的进程， 但是为了降低复杂性，它们都被编译到同一个可执行文件，并在一个进程中运行。

这些控制器包括:

节点控制器（Node Controller）: 负责在节点出现故障时进行通知和响应
任务控制器（Job controller）: 监测代表一次性任务的 Job 对象，然后创建 Pods 来运行这些任务直至完成
端点控制器（Endpoints Controller）: 填充端点(Endpoints)对象(即加入 Service 与 Pod)
服务帐户和令牌控制器（Service Account & Token Controllers）: 为新的命名空间创建默认帐户和 API 访问令牌
cloud-controller-manager
云控制器管理器是指嵌入特定云的控制逻辑的 控制平面组件。 云控制器管理器使得你可以将你的集群连接到云提供商的 API 之上， 并将与该云平台交互的组件同与你的集群交互的组件分离开来。
cloud-controller-manager 仅运行特定于云平台的控制回路。 如果你在自己的环境中运行 Kubernetes，或者在本地计算机中运行学习环境， 所部署的环境中不需要云控制器管理器。

与 kube-controller-manager 类似，cloud-controller-manager 将若干逻辑上独立的 控制回路组合到同一个可执行文件中，供你以同一进程的方式运行。 你可以对其执行水平扩容（运行不止一个副本）以提升性能或者增强容错能力。

下面的控制器都包含对云平台驱动的依赖：

节点控制器（Node Controller）: 用于在节点终止响应后检查云提供商以确定节点是否已被删除
路由控制器（Route Controller）: 用于在底层云基础架构中设置路由
服务控制器（Service Controller）: 用于创建、更新和删除云提供商负载均衡器
Node 组件 
节点组件在每个节点上运行，维护运行的 Pod 并提供 Kubernetes 运行环境。

kubelet
一个在集群中每个节点（node）上运行的代理。 它保证容器（containers）都 运行在 Pod 中。

kubelet 接收一组通过各类机制提供给它的 PodSpecs，确保这些 PodSpecs 中描述的容器处于运行状态且健康。 kubelet 不会管理不是由 Kubernetes 创建的容器。

kube-proxy
kube-proxy 是集群中每个节点上运行的网络代理， 实现 Kubernetes 服务（Service） 概念的一部分。

kube-proxy 维护节点上的网络规则。这些网络规则允许从集群内部或外部的网络会话与 Pod 进行网络通信。

如果操作系统提供了数据包过滤层并可用的话，kube-proxy 会通过它来实现网络规则。否则， kube-proxy 仅转发流量本身。

容器运行时（Container Runtime） 
容器运行环境是负责运行容器的软件。

Kubernetes 支持多个容器运行环境: Docker、 containerd、CRI-O 以及任何实现 Kubernetes CRI (容器运行环境接口)。

插件（Addons） 
插件使用 Kubernetes 资源（DaemonSet、 Deployment等）实现集群功能。 因为这些插件提供集群级别的功能，插件中命名空间域的资源属于 kube-system 命名空间。

下面描述众多插件中的几种。有关可用插件的完整列表，请参见 插件（Addons）。

DNS 
尽管其他插件都并非严格意义上的必需组件，但几乎所有 Kubernetes 集群都应该 有集群 DNS， 因为很多示例都需要 DNS 服务。

集群 DNS 是一个 DNS 服务器，和环境中的其他 DNS 服务器一起工作，它为 Kubernetes 服务提供 DNS 记录。

Kubernetes 启动的容器自动将此 DNS 服务器包含在其 DNS 搜索列表中。

Web 界面（仪表盘）
Dashboard 是 Kubernetes 集群的通用的、基于 Web 的用户界面。 它使用户可以管理集群中运行的应用程序以及集群本身并进行故障排除。

容器资源监控
容器资源监控 将关于容器的一些常见的时间序列度量值保存到一个集中的数据库中，并提供用于浏览这些数据的界面。

集群层面日志
集群层面日志 机制负责将容器的日志数据 保存到一个集中的日志存储中，该存储能够提供搜索和浏览接口。

The Distributed Application Runtime (Dapr) is an open-source, portable, and event-driven runtime. It enables developers to build elastic, stateless/stateful applications running on cloud platforms and edge devices. Dapr can lower the threshold for building modern cloud-native applications based on the microservice architecture.

Why we chose Dapr
At Alibaba, Java is widely used for business applications, various middleware servers, and basic capabilities. Over the past decade, Alibaba has built a solid Java based technology stack in various of battle tested scenarios.

However, as Alibaba’s business growing rapidly and its adoption of cloud-native technology, multi-language requirement, including Node.js, Golang, C, C++, and Rust, etc soon becomes a need. As a result, an efficient solution to develop microservices without language restriction is highly demanded.

Today, more of our business applications, especially frontend services, have begun to adopt FaaS and serverless as application hosting and resource scheduling solutions. However, FaaS and serverless scenarios require a more lightweight solution to meet fast start-up and scaling needs. In the conventional class library mode, a business application becomes bloated because a large number of SDKs must be integrated. It is more inconsistent for applications in the function form. Take Node.js as an example - Hundreds of lines of Node.js function code still need to depend on tens of MB of node modules. FaaS and serverless also pose higher requirements for multi-language support. Therefore, in FaaS and serverless scenarios, it is necessary to provide a more lightweight and multi-language solution different from the conventional class library approach.

Finally, Alibaba chose using a solution with multiple runtimes running as sidecars. A similar approach was also mentioned in “Multi-Runtime Microservices Architecture by Bilgin Ibryam.


Dapr is the first open-source project to practice the multiple runtime concept.Alibaba has paid close attention to Dapr since its release because it had the potential to help solve some of the challenges we’ve encountered. Namely, the sidecar pattern supports multiple languages and so making applications more lightweight with the Dapr runtime replacing the need for client SDKs. This is especially helpful when migrating a product to different environments or deploying it across environments - a process that can be very painful.In this context, the function-oriented programming concept, portable and scalable standard APIs, and platform-neutral and vendor-free design of Dapr makes a lot sense.

Li Xiang, Senior Staff Engineer at Alibaba Cloud, said, “At Alibaba Cloud, we believe that Dapr will lead the way in microservice development. By adopting Dapr, our customers can build portable and robust distributed systems faster.

In the middle of 2020, Alibaba conducted an internal small-scale pilot using Dapr to explore and verify the Dapr’s validity in a real-world implementation.We’ve also been actively participating in the Dapr community and have submitted a large number of improvement suggestions, feedback and code.

In the next section, this article takes the real-world implementation of Dapr in Alibaba as an example to describe how Dapr helps solve the problems described above.

Dapr use cases at Alibaba
Overview
Dapr is still in the experimental stage in Alibaba.Currently, our primary goal is to develop Dapr components for internal middleware.With Dapr components, business applications can be decoupled from this middleware and the Java and Java Client SDK that implements them.Additionally, we are working to verify Dapr in various scenarios by implementing Dapr in several business applications.After the verification, business applications will be deployed on a large scale.

As of March 2021, Dapr has been implemented in two scenarios in Alibaba: multi-language support and cloud-to-cloud migration.


Multi-language support
FaaS and serverless scenarios
The challenge: Alibaba’s e-commerce system involves a large number of requirements for activities and shopping guides.


These requirements are short duration, stable traffic, and fast response.They require rapid development and iteration and have a relatively short life cycle.Therefore, FaaS is suitable for such requirements.


FaaS has strong demands for multi-language support and is not limited to Java. However, most of the applications in Alibaba use the Java system and have adequate multi-language support, especially for emerging languages (such as Dart) or niche languages (such as Rust).

FaaS applications also need to communicate with on-premises services, various middleware, and infrastructures. Therefore, multi-language support is critical for FaaS.

With Dapr, Alibaba has solved the multi-language problem of FaaS and helped customers improve their development efficiency through FaaS.

Multi-language application integration
The challenge: Alibaba acquires a large number of companies, each using different solutions and technology stacks.

The companies acquired by Alibaba have a large number of applications, many of which do not use the Java system.These applications have clear requirements for multi-language support during integration into the Alibaba technical system.For example, Node.js and Golang are required for some applications, while Dart and C++ are required for other applications.


However, the ecosystem of these languages is not as complete as Java.In particular, some middleware and infrastructures are very mature and only need maintenance while others are not.In reality, it is unpractical to redevelop clients in all languages because due to cost and time constraints.

Alibaba can provide multi-language solutions for these applications through Dapr.


Complex Java based legacy systems
The challenge: Complex systems designed based on Java ClassLoader

Alibaba designed complex systems based on ClassLoader for the Java system to solve the class conflict problem and isolate different service modules.The design of these systems is often complex, and applications are bloated.


In addition, some business teams maintain a set of multi-language middleware SDKs to interconnect with existing middleware. These SDKs should be maintained and simultaneously updated by the middleware team. Aside from the effort it requires, this also brings hidden stability risks.

Therefore, Alibaba expects to migrate these legacy systems to Dapr to maintain and update the middleware SDKs uniformly.In addition, we aim to remove the need for business development teams to make code changes to reduce the impact on business applications during migration. When migrating these legacy systems to Dapr, we design a Java adaptation layer, which adapts original Java calls to the Dapr client API.


The implementation of multi-language support in the three scenarios above is illustrated below:



Cloud-to-cloud migration
The challenge: Cross-platform requirements exist when providing business applications externally.


Some Alibaba services, such as DingTalk Document, were originally provided for internal and external Alibaba users.Users only need to deploy DingTalk Document in the internal business cluster of Alibaba, and then they can directly access the Alibaba ecosystem.

However, as the SaaS business develops, the demand for data security from users sensitive to information security also increases. Thus, users want to deploy DingTalk Document in VPCs or the public cloud.

Therefore, the DingTalk Document system need to be migrated to the public cloud.The underlying technologies of DingTalk Document are based on the internal technical system of Alibaba.Now, they need to be migrated to commercial products based on open-source technologies or Alibaba Cloud technologies.


With the standard API and scalable components of Dapr, users can directly shield the underlying middleware through the Dapr runtime without any code modifications.If business systems are deployed on different platforms, consistent capabilities are provided by activating different components in Dapr.

Take message communication as an example. When an application needs to access the message system, users can:


Inside Alibaba: Activate RocketMQ components through Rocketmq.yaml.
On the Public Cloud: Activate Kafka components through Kafka.yaml.
The portability of Dapr decouples the upper-layer applications of DingTalk Document from underlying infrastructures, such as the message system.Thus, DingTalk Document achieves smooth migration between different cloud platforms.

And so, Dapr helps the business team deploy DingTalk anywhere.



Future plans for Dapr at Alibaba
In the future, we will continue to verify Dapr through pilot applications in the following aspects:


Applicable scenarios
Performance
Stability
Portability
We are developing Dapr components to integrate with more middleware and infrastructures, including internal products and commercial products supported on Alibaba Cloud.After passing the verification, Alibaba will contribute the integration code of commercial products on Alibaba Cloud to the Dapr project. By doing so, we can provide support for Dapr, including:


RPC support for Apache Dubbo
Messaging support for Apache RocketMQ
Dynamic configuration support for Nacos
MySQL support for Alibaba Cloud RDS
Redis support for Alibaba Cloud Open Cache Service (OCS)
Alibaba is a pioneer of the multiple runtime architecture and an early adopter of Dapr.We are working with the Dapr community to improve the functions, performance, and stability of Dapr. We are looking forward to continue building the cloud-native Distributed Application Runtime together with the community.
Java 近期新闻综述包括：Jakarta EE 10 推出了一个核心 Profile、JEP 417、JDK 18、Open Liberty 21.0.0.10-beta、Payara 2021 年 8 月路线图更新网络研讨会、Quarkus 2.2.2.Final、一个新的 Micronaut Java 库、Hibernate Search 6.1.0.Alpha1、GraalVM Native Build Tools 0.9.5、Groovy 的版本更新、以及 JakartaOne Livestream 2021 会议。

OpenJDK
JEP 417，Vector API的第三轮孵化，已经从 JDK 18 的“候选”（Candidate）状态提升为“提议目标”（Proposed to Target）状态。除了性能上的改进，该 JEP 还建议合并增强功能，以响应前两轮的孵化反馈：JEP 414（Vector API的第二轮孵化）及 JEP 338（Vector API的第一轮孵化）。JEP 338 已经作为孵化模块集成到了 JDK 16 中，而 JEP 414 也已确认会进入 JDK 17 的最终 JEP。

JDK 17
JDK 17 计划于 2021 年 9 月 14 日（星期二）发布。InfoQ 将持续跟进以进行更详细的新闻报道。

JDK 18
上周，JDK 18早期体验版本的第14版发布了，其中包含了对第 13 版中各种问题的修复更新。更多详细信息请查看发布说明。



对于 JDK 17 和 JDK 18，均鼓励开发人员通过Java Bug Database来提交 Bug。

Jakarta EE 10 的线路
在 Jakarta EE 10发布计划公布后不久，Jakarta EE工作组推出了新的 Jakarta EE核心Profile（Core Profile），以补充现有的平台Profile（Platform Profile）和Web Profile。这个新的 Profile“专注于为适用于微服务的小型运行时提供最小的基础，并允许提前编译。”如下所示，已经为该核心 Profile 定义了一组 Jakarta EE 规范：




Open Liberty
IBM发布了OpenLiberty 21.0.0.10-beta 版，该版本支持 JDK 17早期体验版本的第35版；OpenID Connect Client 1.0和Social Media Login 1.0能够接收 JSON Web 加密（JWE）格式的令牌；支持 MicroProfile Context Propagation 1.3-RC1；并完实现了 Jakarta EE 9.0 的增值特性，如Admin Center、gRPC和Web服务安全。

Payara
Payara已经召开了2021年8月路线图更新网络研讨会，该研讨会由 Payara 首席执行官Steve Millidge主持。在会上，他讨论回顾了 2021 年的路线图；到目前为止，Payara 在 2021 年交付了什么；目前正在开发的项目；以及 Payara Cloud 的更新。

Quarkus
Red Had发布了一个维护版本，Quarkus 2.2.2.Final，其特性是升级到了Oracle JDBC驱动程序21.3.0.0版；以及通过 Quarkus 扩展，以编程方式传递在 GraalVM 中引入--exclude-config选项的能力。更多详细信息请查看变更日志。

Micronaut
Object Computing, Inc.的首席软件工程师Sergio Del Amo推出了一个Micronaut Java 库来使用Pushover API，该 API 既可用于 Micronaut 应用程序，也可作为独立库使用。Del Amo 在此GitHub仓库中提供了关于如何实现每个场景的示例。

Hibernate
Hibernate Search 6.1.0.Alpha1 已经发布，其特性包括：引入了一个新的异步、分布式自动索引概念，并对Hibernate ORM、Lucene和Elasticsearch进行了依赖升级。

JakartaOne Livestream 2021 会议
JakartaOne Livestream 2021会议的论文征集将于 2021 年 9 月 15 日结束。会议定于 2021 年 12 月 7 日召开，今年的项目委员会成员包括：Eclipse 的 Jakarta EE 项目经理Tanja Obradovic、Eclipse 的 Jakarta EE 开发人员倡导者Ivar Grimstad、xgeeks 的顾问软件工程师Otavio Santana、VIDA Software 的高级程序员Ivan St.Ivanov、JetBrains 的 Java 开发人员倡导者Dalia Abo Sheasha、Sensor Aktor GmbH 的董事总经理 Jan Westerkamp、以及某石化研究公司的高级研究技术员Michael Redlich。

GraalVM
作为 1.0 版本的一个重要里程碑，Oracle实验室发布了 Native Build Tools 的0.9.5版，这是一个 GraalVM 项目，包含了用于与 GraalVM Native Image互操作的插件。 该版本弃用了 nativeBuild和nativeTest扩展，支持了graalvmNative ，如下所示：
";

        public override async Task RunAsync(CancellationToken cancellationToken = default)
        {
            using var client = new DaprClientBuilder( )
                .UseHttpEndpoint("http://localhost:3500")
                .UseGrpcEndpoint("http://localhost:50000")
              .Build();

            var data = new WeatherForecast()
            {
                Date = DateTime.Now,
                TemperatureC = 12,
                Summary = "Sunny",
                Data = Encoding.UTF8.GetBytes($"二进制数据传输，当 kubectl 基于非 ASCII 或 UTF-8 的输入创建 ConfigMap 时， 该工具将这些输入放入 ConfigMap 的 binaryData 字段，而不是 data 中。 同一个 ConfigMap 中可同时包含文本数据和二进制数据源。{Data}")

            };

            await client.InvokeBindingAsync(
                            bindingName,
                            operation,
                            Encoding.UTF8.GetBytes(Data),
                            cancellationToken: cancellationToken);

            Console.WriteLine("Message has been sent !");
        }
    }
}
