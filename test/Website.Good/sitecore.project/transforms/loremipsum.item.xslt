<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
    <xsl:output method="xml" indent="yes" />

    <xsl:template match="LoremIpsum">
        <Item xmlns="http://www.sitecore.net/pathfinder/item" Template="/sitecore/templates/Sample/XmlTransformTemplate" Template.CreateFromFields="True">
            <Fields>
                <Field Name="Title">Lorem Ipsum</Field>
                <Field Name="Text">Lorem Ipsum</Field>
            </Fields>
        </Item>
    </xsl:template>
</xsl:stylesheet>