Cofoundry uses a fairly simple permissions system. At the most basic level a *permission* represents a single action and a *role* represents a collection of many permitted permissions. Roles are assigned to users, or if you are not signed in then a special *Anonymous* role is assigned to you.

As with other roles, the *Anonymous* role is assigned a set of permissions which you can customize to the requirements of your application, but by default the role is assigned read-only access to most entities to allow basic website functions to work.

## In this section:

- [Managing Roles](Managing-Roles): Managing roles via the admin panel or in code.
- [Creating Permissions](Creating-Permissions): How to create your own custom permissions.
- [Authorizing CQS Handlers](Authorizing-CQS-Handlers): Applying authorization to command and queries in the CQS framework.
- [Authorizing Routes](Authorizing-Routes): Applying authorization to controllers and other routing mechanisms.
- [Querying & Validating in Code](Quering-and-Validating-in-Code): Querying and validating permissions in code.
- [Querying in Views](Quering-in-Views): Querying permissions and roles in Razor views.

