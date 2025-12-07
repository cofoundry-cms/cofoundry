# Cofoundry Samples

This directory contains several sets of samples:

- Samples from the [Cofoundry.Samples](https://github.com/cofoundry-cms/Cofoundry.Samples) repo. This repo is the source of truth for thes samples. They are copied into the official samples repo by running the [/eng/Cofoundry.SamplesGenerator](/eng/Cofoundry.SamplesGenerator) project.
- Samples for various plugin packages. These are not currently documented as official samples, but are used for development.
- Projects used only in the development of Cofoundry e.g. the [Dev/Dev.Sandbox](Dev/Dev.Sandbox) project.

## Getting Started

Each sample is configured to use a local database provisioned via the docker compose file located in the [local-env](local-env/) directory. Run the docker compose file before starting any of the sample sites. For more information on running docker compose see the [local-env docs](local-env/README.md).
