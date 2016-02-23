# Chauffeur #
A service that communicates with the Jenkins Continious Integration Service to download and install artifacts on the host servers. 

## Windows Service ##
A windows service (that hosts a WCF Service) that allows communication between the Jenkins build server and the machine that hosts the window service to automatically
install the last successful build on the machine.

## Groovy Script ##
A groovy script that can be configured in a "post-build" event that will notify the specified servers that a new build needs to be installed.

### Requirements ###
- 4.5 .NET Framework
- Visual Studio 2013

### Third Party Libraries ###
- https://jenkins-ci.org/ 


## Getting Started ##

1. Download and install the Jenkins Chauffeur Windows Service.
2. Configure the service by navigating to the `Chauffeur.exe.config` file.

    - `Chauffeur/Resources/Templates` - The path to the template directory.
    - `Chauffeur/Resources/Data` - The path to the data directory.
    - `Chauffeur/Resources/Packages` - The path to the json file contains the installed packages.
    - `Chauffeur/Jenkins/Server` - The URL to the Jenkins CI.
    - `Chauffeur/Jenkins/User` - The name of the user that has access to the Jenkins CI.
    - `Chauffeur/Jenkins/Token` - The API token for the user.
    - `Chauffeur/Packages/Artifacts` - The path to the directory that will contain the downloaded artifacts for the builds.    
    - `Chauffeur/Packages/PreInstall` - The path to an powershell script file that will be run before the uninstall of the MSI.
    - `Chauffeur/Packages/PostInstall` - The path to an powershell script file that will be run after the install of the MSI.
    - `Chauffeur/Notifications/Host` - The STMP server.
    - `Chauffeur/Notifications/To` - The group alias or individual e-mail addresses separated by commas.
    - `Chauffeur/Notifications/From` - The group alias or e-mail address.    
    - `Chauffeur/Notifications/Subject` - The path to the XSLT used to transform the `package.xml` into readable format.
    - `Chauffeur/Notifications/Body` - The path to the XSLT used to transform the `package.xml` into readable format.

3. Start and stop the service to allow the latest configuration changes to take affect.
4. Download and install the `Groovy Postbuild` plugin on the Jenkins server.
5. Copy and past the contents of the `Jenkins.groovy` script into the contents window of the `Groovy Postbuild` plugin on the build configuration (that is the highest in build chain that generates an MSI).    

    ```groovy
    /*
        .SYNOPSIS
            The script is designed to act as a client proxy that will notify the
            machines (that are hosting the Jenkins Chauffeur Service) that a new
            build should be installed.
        .NOTES
            File Name      : Chauffeur.groovy
            Author         : Kyle Baesler
            Prerequisite   : Groovy 1.8.9
                           : Groovy Postbuild Plugin (https://wiki.jenkins-ci.org/display/JENKINS/Groovy+Postbuild+Plugin)
     */

    import groovy.json.JsonSlurper

    // The name of the computers that host the Chauffeur service.
    def MACHINE_NAMES = ["localhost"]

    // The port that the WCF service is hosted on.
    def PORT = 8080

    def result = manager.getResult()
    manager.listener.logger.println('Chauffeur.groovy: ' + result)

    if (result != "SUCCESS") {
        return;
    }

    def jobName = manager.envVars['JOB_NAME']
    def buildNumber = manager.envVars['BUILD_NUMBER']

    def passes = new ArrayList<String>()
    def errors = new ArrayList<String>()

    MACHINE_NAMES.eachWithIndex { String s, int i ->
        try {

            def url = new URL("http://" + s + ":" + PORT + "/Chauffeur.Jenkins.Services/ChauffeurService/rest/Install/" + jobName + "/" + buildNumber)
            manager.listener.logger.println('Chauffeur.groovy: ' + url)

            def text = url.getText()
            manager.listener.logger.println('Chauffeur.groovy: ' + text)

            def jsonSlurper = new JsonSlurper()
            def build = jsonSlurper.parseText(text);
            if (build != null && build.number.toString() == buildNumber) {
                passes.add(s)
                manager.listener.logger.println("Chauffeur.groovy: Successfully installed on " + s);
            } else {
                errors.add(s)
                manager.listener.logger.println("Chauffeur.groovy: Failed to install on " + s);
            }
        } catch (Exception e) {
            errors.add(s)
            manager.listener.logger.println("Chauffeur.groovy: " + e.message)
        }
    }

    if (errors.size() > 0)
        throw new Exception()

    manager.addInfoBadge(passes.join(", "))
    ```
    > The script assumes that the WCF configurations for the service in the `Chauffeur.exe.config` have not been modified. 
