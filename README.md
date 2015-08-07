[![Build status](https://ci.appveyor.com/api/projects/status/c46okqadhrke0at7/branch/master?svg=true)](https://ci.appveyor.com/project/Jumpercables/chauffeur/branch/master)
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

1. Download and install the Jenkins Chauffuer Windows Service.
2. Setup the service by navigating to the `Chauffuer.exe.config` file.

    - `Chauffuer/Jenkins/Server` - The URL to the Jenkins CI.
    - `Chauffuer/Jenkins/User` - The name of the user that has access to the Jenkins CI.
    - `Chauffuer/Jenkins/Token` - The API token for the user.

    - `Chauffuer/Packages/DataDirectory` - The path to the directory that will contain the downloaded artifacts for the builds.
    - `Chauffuer/Packages/InstallPropertyReferences` - The property references that are passed to the MSI during install.
    - `Chauffuer/Packages/UninstallPropertyReferences` - The property references that are passed to the MSI during uninstall.

    - `Chauffeur/Notification/Host` - The STMP server.
    - `Chauffeur/Notification/To` - The group alias or individual e-mail addresses separated by commas.
    - `Chauffeur/Notification/From` - The group alias or e-mail address.
    - `Chauffeur/Notification/IsHtml` - When `true` the body can be expressed as HTML.
    - `Chauffeur/Notification/Subject` - The subject of the e-mail.
    - `Chauffeur/Notification/Body` - The body of the e-mail.