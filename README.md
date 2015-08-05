[![Build status](https://ci.appveyor.com/api/projects/status/c46okqadhrke0at7/branch/master?svg=true)](https://ci.appveyor.com/project/Jumpercables/chauffeur/branch/master)
[![Stories in Backlog](https://badge.waffle.io/Jumpercables/Chauffeur.svg?label=backlog&title=Backlog)](http://waffle.io/Jumpercables/Chauffeur)
[![Stories in Ready](https://badge.waffle.io/Jumpercables/Chauffeur.svg?label=ready&title=Ready)](http://waffle.io/Jumpercables/Chauffeur)

# Chauffeur #
An extension to the Jenkins CI that allows for automatically updating test machines with the latest build.

## Jenkins Chauffeur Service ##
A windows service (that hosts a WCF Service) that allows communication between the Jenkins build server and the machine that hosts the window service to automatically
install the last successful build on the machine.

## Jenkins Chauffeur Client ##
A console application that notifies the Jenkins Chauffeur Service that a successful build has completed and should be installed, which needs to be configured as a post-build event in Jenkins.

### Requirements ###
- 4.5 .NET Framework
- Visual Studio 2013

### Third Party Libraries ###
- https://jenkins-ci.org/ 


## Getting Started ##

1. Install the Chauffuer Windows Service on the machine that will have the builds installed and uninstalled.

2. Change the Jenkins keys in the `Chauffeur.exe.config` file that is installed with the Chauffeur Windows Service.

    - `jenkins.server` - The URL to the Jenkins CI.
    - `jenkins.user` - The name of the user that has access to the Jenkins CI.
    - `jenkins.token` - The API token for the user.     

3. Change the Chauffeur keys in the `Chauffeur.exe.config` file that is installed with the Chauffeur Windows Service.

    - `chauffeur.packages` - The path to the directory that will contain the downloaded artifacts for the builds `(default %COMMONAPPDATA%\Jenkins)`
    - `chauffeur.install` - The parameters that are passed during install `(default /quiet)`.
    - `chauffeur.uninstall` - The parameters that are passed during uninstall `(default /quiet)`.     