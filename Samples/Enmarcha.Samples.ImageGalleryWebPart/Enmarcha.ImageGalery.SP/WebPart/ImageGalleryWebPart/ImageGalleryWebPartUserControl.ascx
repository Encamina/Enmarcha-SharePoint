<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageGalleryWebPartUserControl.ascx.cs" Inherits="Enmarcha.ImageGalery.SP.WebPart.ImageGalleryWebPart.ImageGalleryWebPartUserControl" %>
<script type="text/javascript" src="/Style%20Library/js/jquery-1.4.2.min.js"  ></script>
<script type="text/javascript" src="/Style%20Library/js/jquery.liquidcarousel.min.js"  ></script>
<script type="text/javascript" src="/Style%20Library/js/imagegallery/init.js"  ></script>
<link rel="stylesheet" type="text/css" href="/Style%20Library/css/liquidcarousel.css" />
<link rel="stylesheet" type="text/css" href="/Style%20Library/css/style.css" />
<div class="liquid">
	<span class="previous"></span>
	<div class="wrapper">
		<ul>
		      <asp:ListView ID="listViewImageGalery" runat="server">
                <LayoutTemplate>
                    <asp:PlaceHolder ID="ItemPlaceHolder" runat="server"></asp:PlaceHolder>
                </LayoutTemplate>
                <ItemTemplate>
                   <li><a href="<%#Eval("NewUrl") %>" <%#Eval("TargetBlank") %>><img src="<%#Eval("PictureUrl") %>" width="88" height="126" alt="image"/></a></li>        
                </ItemTemplate>
                <EmptyDataTemplate>
                </EmptyDataTemplate>
            </asp:ListView> 						
		</ul>
	</div>
	<span class="next"></span>
</div>