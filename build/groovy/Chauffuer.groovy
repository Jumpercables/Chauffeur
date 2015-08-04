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
        Chauffeur.groovy Chauffeur_Nightly MACHINE_1 MACHINE_2 LOCALHOST
 */
class Chauffuer {
    static void main(String... args) {

        // There must be at least two arguments.
        if(!args || args.length < 2){
            println('Chauffuer.groovy [jobName] [machineName]')
            return
        }

        try {
            args.eachWithIndex { String s, int i ->

                // Assumption: The 1st argument is the job name.
                if(i > 0)
                {
                    def url = new URL('http://' + s + ':8080/Chauffeur.Jenkins.Services/ChauffeurService/rest/Install/' + args[0])
                    def text = url.getText()
                    println(text)
                }
            }
        } catch (Exception e) {
            e.printStackTrace()
        }
    }
}