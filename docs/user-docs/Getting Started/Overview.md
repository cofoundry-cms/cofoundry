Cofoundry is an open source .NET application framework and content management platform. Whilst content management and website tools are a large part of Cofoundry, the functionality is entirely optional and our base framework is available to you to use in any application that references .NET 6.

We see the role of developing an application as very separate from managing content and so we don't squeeze everything into the same management GUI, instead we focus on modular and extensible code-first tools for building sites and a simple, clean interface for managing them.

## Components

![Cofoundry component overview](images/overview.png)

### Content Management

Cofoundry is designed to work with a wide range of content. We have the usual [pages](/content-management/pages), [images](/content-management/images) and [documents](/content-management/documents) you'd expect from a CMS, but we also include a flexible [custom entity framework](/content-management/custom-entities) that lets you easily define data types like products, stores, categories or events.

Page templates are just ASP.NET Razor views with strongly typed view modules so you have complete freedom to develop the front-end however you want. Editable content regions can be added anywhere and configured with content type restrictions. This gives you a sliding scale as to whether you give content editors free-form flexibility to create highly customized pages or lock them down so they can't break your designs. 

Content editors benefit from inline page editing which makes it really easy to see what's being changed and what impact it will have on the design and flow of the page.

### Framework Tools

Cofoundry includes many tools, helpers and abstractions to solve problems that are common to many applications such as sending [mail](/framework/mail), running [background tasks](/framework/background-tasks), [applying updates automatically](/framework/auto-update) and managing files. 

Also included are tools and helpers that represent our approach to structuring code such as our [CQS](/framework/data-access/CQS) framework, modular registrations and WebApi helpers. We realize however that development style and architecture can be a personal choice and so we keep their use optional.

The aim is for Cofoundry to augment your existing development skills, without getting in the way of building an application with the tools and methodologies you know.


### Extensibility Platform

It's impossible to build a framework that accounts for all eventualities and so with Cofoundry we make it easy for you to integrate your own tools and to extend our base framework.

[Dependency injection](/framework/dependency-injection) is used everywhere which not only lets you replace any of our implementations, but also lets you inject your favorite tools into our framework.

We also have extensibility points that let you modularize your code and easily build self-registering [plugins](/plugins/creating-a-plugin) that extend or override behavior without requiring complicated configuration. For example you can use plugins to make your site cloud compatible, swap in your preferred logging framework or add extra features to your site.

## Getting to know Cofoundry

#### Using the sample projects

To get a flavor for how Cofoundry works, we suggest you check out one of the [Sample Projects](Sample-Projects). This will give you an idea of how all the basic parts sit together before you get started on your own implementation.

#### Installing Cofoundry

Cofoundry and Cofoundry Plugins are installed via [Nuget](https://www.nuget.org/packages?q=Cofoundry). Check out the [installation guide](installing) for more information on how to get up and running.

#### Community & Feedback

Need help, got a question or want to give us some feedback? We're keen to hear from you! Find out [how to get in touch](https://www.cofoundry.org/contact)