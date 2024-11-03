<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet
  version="1.0"
  xmlns:sm="http://www.sitemaps.org/schemas/sitemap/0.9"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:mobile="http://www.google.com/schemas/sitemap-mobile/1.0"
  xmlns:image="http://www.google.com/schemas/sitemap-image/1.1"
  xmlns:video="http://www.google.com/schemas/sitemap-video/1.1"
  xmlns:news="http://www.google.com/schemas/sitemap-news/0.9"
  xmlns:fo="http://www.w3.org/1999/XSL/Format"
  xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" doctype-system="about:legacy-compat" indent="yes" encoding="UTF-8"/>

  <xsl:template match="/">
    <html>
      <head>
        <meta charset="utf-8" />
        <title>
          <xsl:if test="sm:urlset/sm:url/mobile:mobile">Mobile </xsl:if>
          <xsl:if test="sm:urlset/sm:url/image:image">Images </xsl:if>
          <xsl:if test="sm:urlset/sm:url/news:news">News </xsl:if>
          <xsl:if test="sm:urlset/sm:url/video:video">Video </xsl:if>
          XML Sitemap
          <xsl:if test="sm:sitemapindex"> Index</xsl:if>
        </title>
        <link rel="shortcut icon" href="/favicon.ico" />
      </head>
      <body>
        <div id="wrapper">
          <header>
            <h1>
              <xsl:if test="sm:urlset/sm:url/mobile:mobile">Mobile </xsl:if>
              <xsl:if test="sm:urlset/sm:url/image:image">Images </xsl:if>
              <xsl:if test="sm:urlset/sm:url/news:news">News </xsl:if>
              <xsl:if test="sm:urlset/sm:url/video:video">Video </xsl:if>
              XML Sitemap<xsl:if test="sm:sitemapindex"> Index</xsl:if>
            </h1>
            <h2>
              <xsl:choose>
                <xsl:when test="sm:sitemapindex">
                  Total Files: <xsl:value-of select="count(sm:sitemapindex/sm:sitemap)"/>
                </xsl:when>
                <xsl:otherwise>
                  Total URLs: <xsl:value-of select="count(sm:urlset/sm:url)"/>
                </xsl:otherwise>
              </xsl:choose>
            </h2>
          </header>

          <div id="main">

            <h3>XML sitemap URLs</h3>

            <xsl:apply-templates />
          </div>
        </div>

      </body>
    </html>
  </xsl:template>

  <xsl:template match="sm:sitemapindex">
    <table cellpadding="0" cellspacing="0" width="100%">
      <thead>
        <tr>
          <th>#</th>
          <th>URL</th>
          <th>Last Modified</th>
        </tr>
      </thead>
      <tbody>
        <xsl:for-each select="sm:sitemap">
          <tr>
            <xsl:variable name="loc">
              <xsl:value-of select="sm:loc"/>
            </xsl:variable>
            <xsl:variable name="pno">
              <xsl:value-of select="position()"/>
            </xsl:variable>
            <td>
              <xsl:value-of select="$pno"/>
            </td>
            <td>
              <a href="{$loc}">
                <xsl:value-of select="sm:loc"/>
              </a>
            </td>
            <xsl:apply-templates/>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="sm:urlset">
    <table cellpadding="0" cellspacing="0" width="100%">
      <thead>
        <tr>
          <th>#</th>
          <th>URL</th>
          <xsl:if test="sm:url/sm:lastmod">
            <th>Last Modified</th>
          </xsl:if>
          <xsl:if test="sm:url/sm:changefreq">
            <th>Change Frequency</th>
          </xsl:if>
          <xsl:if test="sm:url/sm:priority">
            <th>Priority</th>
          </xsl:if>
        </tr>
      </thead>
      <tbody>
        <xsl:for-each select="sm:url">
          <tr>
            <xsl:variable name="loc">
              <xsl:value-of select="sm:loc"/>
            </xsl:variable>
            <xsl:variable name="pno">
              <xsl:value-of select="position()"/>
            </xsl:variable>
            <td>
              <xsl:value-of select="$pno"/>
            </td>
            <td>
              <a href="{$loc}">
                <xsl:value-of select="sm:loc"/>
              </a>
            </td>
            <xsl:apply-templates select="sm:*"/>
          </tr>
          <xsl:apply-templates select="image:*"/>
          <xsl:apply-templates select="video:*"/>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="sm:loc|image:loc|image:caption|video:*">
  </xsl:template>

  <xsl:template match="sm:lastmod|sm:changefreq|sm:priority">
    <td>
      <xsl:apply-templates/>
    </td>
  </xsl:template>

  <xsl:template match="image:image">
    <tr>
      <xsl:variable name="loc">
        <xsl:value-of select="image:loc"/>
      </xsl:variable>
      <td></td>
      <td class="url2">
        <a href="{$loc}">
          <xsl:value-of select="image:loc"/>
        </a>
      </td>
      <td colspan="5">
        <div style="width:400px">
          <xsl:value-of select="image:caption"/>
        </div>
      </td>
      <xsl:apply-templates/>
    </tr>
  </xsl:template>
  <xsl:template match="video:video">
    <tr>
      <xsl:variable name="loc">
        <xsl:choose>
          <xsl:when test="video:player_loc != ''">
            <xsl:value-of select="video:player_loc"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="video:content_loc"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:variable>
      <xsl:variable name="thumb">
        <xsl:value-of select="video:thumbnail_loc"/>
      </xsl:variable>
      <td></td>
      <td class="url2">
        <a href="{$loc}">
          <xsl:choose>
            <xsl:when test="video:player_loc != ''">
              <xsl:value-of select="video:player_loc"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="video:content_loc"/>
            </xsl:otherwise>
          </xsl:choose>
        </a>
      </td>
      <td colspan="5">
        <div style="width:400px">
          <xsl:value-of select="video:title"/>
        </div>
        <xsl:if test="video:thumbnail_loc != ''">
          <img src="{$thumb}" alt="" />
        </xsl:if>
      </td>
      <xsl:apply-templates/>
    </tr>
  </xsl:template>

</xsl:stylesheet>