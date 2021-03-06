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

def result = 'SUCCESS'
println('Chauffeur.groovy: ' + result)

if (result != "SUCCESS") {
    return;
}

def jobName = 'The_Name-Of-The-Build'
def buildNumber = 'The-Build-Number'

def passes = new ArrayList<String>()
def errors = new ArrayList<String>()

MACHINE_NAMES.eachWithIndex { String s, int i ->
    try {

        def url = new URL("http://" + s + ":" + PORT + "/Chauffeur.Jenkins.Services/ChauffeurService/rest/Install/" + jobName + "/" + buildNumber)
        println('Chauffeur.groovy: ' + url)

        def text = url.getText()
        println('Chauffeur.groovy: ' + text)

        def jsonSlurper = new JsonSlurper()
        def build = jsonSlurper.parseText(text);
        if (build != null && build.number.toString() == buildNumber) {
            passes.add(s)
            println("Chauffeur.groovy: Successfully installed on " + s);
        } else {
            errors.add(s)
            println("Chauffeur.groovy: Failed to install on " + s);
        }
    } catch (Exception e) {
        errors.add(s)
        println("Chauffeur.groovy: " + e.message)
    }
}

if (errors.size() > 0)
    throw new Exception()
