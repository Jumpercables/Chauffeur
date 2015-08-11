/*
    .SYNOPSIS
        The script is designed to act as a client proxy that will notify the
        machines (that are hosting the Jenkins Chauffuer Service) that a new
        build should be installed.
    .DESCRIPTION
        <jobName> - The name of the job in Jenkins that will be installed.
        <machineName> - The machine names of the computers hosting the Jenkins Chauffuer Service.
    .NOTES
        File Name      : Chauffeur.groovy
        Author         : Kyle Baesler
        Prerequisite   : Groovy 1.8.9
    .EXAMPLE
        Chauffeur.groovy
 */

import hudson.scm.*

class Chauffuer {

    // The name of the job in the build environment.
    static final def JOB_NAME = ""

    // The name of the computers that host the Chauffuer service.
    static final def MACHINE_NAMES = []

    static void main(String... args) {

        // The name of the job in the build.
        if(JOB_NAME == "" || JOB_NAME == null) {
            println("The <JOB_NAME> must be specified.")
            return
        }

        // The name of the machines that should be notified.
        if(MACHINE_NAMES == null || MACHINE_NAMES.size() == 0) {
            println("The <MACHINE_NAMES> must be specified.")
            return
        }

        try {
            MACHINE_NAMES.eachWithIndex { String s, int i ->
                def url = new URL('http://' + s + ':8080/Chauffeur.Jenkins.Services/ChauffeurService/rest/InstallLastSuccesfulBuild/' + JOB_NAME)
                def text = url.getText()
                println(text)
            }
        } catch (Exception e) {
            e.printStackTrace()
        }
    }
}