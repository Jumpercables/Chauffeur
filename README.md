[![Build status](https://ci.appveyor.com/api/projects/status/c46okqadhrke0at7/branch/master?svg=true)](https://ci.appveyor.com/project/Jumpercables/chauffeur/branch/master)
[![Stories in Ready](https://badge.waffle.io/Jumpercables/Chauffeur.svg?label=ready&title=Ready)](http://waffle.io/Jumpercables/Chauffeur)

# Chauffeur #
An extension to the Jenkins CI that allows for automatically updating test machines with the latest build.

## Jenkins Chauffeur Service ##
A windows service (that hosts a WCF Service) that allows communication between the Jenkins build server and the machine that hosts the window service to automatically
install the last successful build on the machine.

## Jenkins Chauffeur Groovy ##
A groovy script that can be configured in a "post-build" event that will notify the specified servers that a new build needs to be installed.

### Requirements ###
- 4.5 .NET Framework
- Visual Studio 2013

### Third Party Libraries ###
- https://jenkins-ci.org/ 


## Getting Started ##

1. Download and install the Jenkins Chauffeur Windows Service.
2. Configure the service by navigating to the `Chauffeur.exe.config` file.

    #### The RESOURCE configurations ####

    - `Chauffeur/Resources/Templates` - The path to the template directory.
    - `Chauffeur/Resources/Data` - The path to the data directory.
    - `Chauffeur/Resources/Packages` - The path to the json file contains the installed packages.

    #### The JENKINS configurations ####

    - `Chauffeur/Jenkins/Server` - The URL to the Jenkins CI.
    - `Chauffeur/Jenkins/User` - The name of the user that has access to the Jenkins CI.
    - `Chauffeur/Jenkins/Token` - The API token for the user.

    #### The PACKAGES installation and de-installation configurations ####
    - `Chauffeur/Packages/Artifacts` - The path to the directory that will contain the downloaded artifacts for the builds.
    - `Chauffeur/Packages/InstallPropertyReferences` - The property references that are passed to the MSI during install.
    - `Chauffeur/Packages/UninstallPropertyReferences` - The property references that are passed to the MSI during uninstall.

    ####   The NOTIFICATION configurations ####
    - `Chauffeur/Notification/Host` - The STMP server.
    - `Chauffeur/Notification/To` - The group alias or individual e-mail addresses separated by commas.
    - `Chauffeur/Notification/From` - The group alias or e-mail address.    
    - `Chauffeur/Notification/Subject` - The path to the XSLT used to transform the `package.xml` into readable format.
    - `Chauffeur/Notification/Body` - The path to the XSLT used to transform the `package.xml` into readable format.

3. Start and stop the service to allow the latest configuration changes to take affect.
4. Download and install the `Groovy Postbuild` plugin on the Jenkins server.
5. Copy and past the contents of the `Chauffeur.groovy` script into the contents window of the `Groovy Postbuild` plugin on the build configuration (that is the highest in build chain that generates an MSI).

    ```
    /*
    .SYNOPSIS
        The script is designed to act as a client proxy that will notify the
        machines (that are hosting the Jenkins Chauffeur Service) that a new
        build should be installed.    
     */

    // The name of the job in the build environment.
    def JOB_NAME = ""

    // The name of the computers that host the Chauffeur service.
    def MACHINE_NAMES = [""]

    // The name of the job in the build.
    if (JOB_NAME == "" || JOB_NAME == null) {
        println("The <JOB_NAME> must be specified.")
        return
    }

    // The name of the machines that should be notified.
    if (MACHINE_NAMES == null || MACHINE_NAMES.size() == 0) {
        println("The <MACHINE_NAMES> must be specified.")
        return
    }

    try {
        MACHINE_NAMES.eachWithIndex { String s, int i ->
            def url = new URL('http://' + s + ':8080/Chauffeur.Jenkins.Services/ChauffeurService/rest/InstallLastSuccessfulBuild/' + JOB_NAME)
            def text = url.getText()
            println(text)
        }
    } catch (Exception e) {
        println(e.printStackTrace())
    }
    ```