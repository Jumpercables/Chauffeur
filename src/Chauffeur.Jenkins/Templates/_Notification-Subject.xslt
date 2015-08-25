<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:fo="http://www.w3.org/1999/XSL/Format"
                xmlns:m="http://schemas.datacontract.org/2004/07/Chauffeur.Jenkins.Model">
  <xsl:output method="text" omit-xml-declaration="yes" encoding="UTF-8" indent="no" />
  <xsl:template match="/package">New Build  <xsl:value-of select="buildNumber"/> Installed</xsl:template>
</xsl:stylesheet>
