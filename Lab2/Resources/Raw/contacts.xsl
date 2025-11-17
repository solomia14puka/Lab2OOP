<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<meta charset="UTF-8" />
				<title>Міжнародні контакти</title>
				<style>
					body { font-family: system-ui, sans-serif; margin: 1rem; }
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid #ccc; padding: .5rem; text-align: left; }
					th { background: #f0f0f0; }
				</style>
			</head>
			<body>
				<h1>Міжнародні контакти</h1>
				<table>
					<thead>
						<tr>
							<th>ID</th>
							<th>П.І.П.</th>
							<th>Факультет</th>
							<th>Кафедра</th>
							<th>Спеціальність</th>
							<th>Вид співпраці</th>
							<th>Часові рамки</th>
							<th>Атрибути</th>
						</tr>
					</thead>
					<tbody>
						<xsl:for-each select="contacts/contact">
							<tr>
								<td>
									<xsl:value-of select="@id" />
								</td>
								<td>
									<xsl:value-of select="name" />
								</td>
								<td>
									<xsl:value-of select="faculty" />
								</td>
								<td>
									<xsl:value-of select="department" />
								</td>
								<td>
									<xsl:value-of select="specialty" />
								</td>
								<td>
									<xsl:choose>
										<xsl:when test="collaboration!=''">
											<xsl:value-of select="collaboration" />
										</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="@collaboration" />
										</xsl:otherwise>
									</xsl:choose>
								</td>
								<td>
									<xsl:value-of select="timeframe" />
								</td>
								<td>
									<xsl:for-each select="@*">
										<xsl:value-of select="name()" />=<xsl:value-of select="." />
										<xsl:if test="position()!=last()">; </xsl:if>
									</xsl:for-each>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>