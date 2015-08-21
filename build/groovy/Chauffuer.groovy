/*
    .SYNOPSIS
        The script is designed to act as a client proxy that will notify the
        machines (that are hosting the Jenkins Chauffeur Service) that a new
        build should be installed.
    .NOTES
        File Name      : Chauffeur.groovy
        Author         : Kyle Baesler
        Prerequisite   : Groovy 1.8.9
    .EXAMPLE
        Chauffeur.groovy
 */

// The name of the job in the build environment.
def JOB_NAME = ""

// The name of the computers that host the Chauffeur service.
def MACHINE_NAMES = []

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