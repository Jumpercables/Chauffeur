[![Build status](https://ci.appveyor.com/api/projects/status/c46okqadhrke0at7?svg=true)](https://ci.appveyor.com/project/Jumpercables/chauffeur)

# Chauffeur #
An extension to the Jenkins CI that allows for automatically updating test machines with the latest build.

## Jenkins Chauffeur Service ##
A windows service (that hosts a WCF Service) that allows communication between the Jenkins build server and the machine that hosts the window service to automatically
install the last successful build on the machine.

## Jenkins Chauffeur Client ##
A console application that notifies the Jenkins Chauffeur Service that a successful build has completed and should be installed.

### Requirements ###
- 4.5 .NET Framework
- Visual Studio 2010+

### Third Party Libraries ###
- https://jenkins-ci.org/ 
