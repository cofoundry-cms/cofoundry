/*
	#175 Pages: Custom data fields
	#176 Users: Custom data fields
*/

alter table Cofoundry.PageVersion add SerializedExtensionData nvarchar(max) null
alter table Cofoundry.[User] add SerializedExtensionData nvarchar(max) null
alter table Cofoundry.ImageAsset add SerializedExtensionData nvarchar(max) null
alter table Cofoundry.DocumentAsset add SerializedExtensionData nvarchar(max) null