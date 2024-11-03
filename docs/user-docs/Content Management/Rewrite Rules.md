*NB the Rewrite Rules functionality currently has no user interface, you'll need to insert rules into the Cofoundry.RewriteRule table manually and refresh the cache to see the changes. Tracked in [issue 143](https://github.com/cofoundry-cms/cofoundry/issues/143)*

Rewrite rules allow you to redirect requests when a page no longer exists. This runs as part of the catch-all dynamic routing, so if a route exists it will always be found and rewrites will only take place as the last step of the routing.

Note that using host-level redirects such as the IIS RewriteRules module will always be faster, so you may prefer to use this if performance is an issue, whereas Cofoundry rewrite rules are great for easy on-the-fly configuration.