# OpenTTDAdminPort



[![codecov](https://codecov.io/gh/shoter/OpenTTDAdminPort/branch/master/graph/badge.svg)](https://codecov.io/gh/shoter/OpenTTDAdminPort)
![github actions](https://github.com/shoter/OpenTTDAdminPort/workflows/Continous%20Integration/badge.svg)



## Features

* Recovery mechanism in case of error - client will try to reconnect after every error.
* Client sends ping messages every minute in order to ensure that connection is still present - aka watchdog.



## Development

### Conventional commits

This repository uses [Conventional commits](https://www.conventionalcommits.org/en/v1.0.0/) to specify type of commits commited to the repository.

Used types:
- feat - feature
- fix
- ref - refactor
- docs - documentation
- chore
