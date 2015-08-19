# graph-org-explorer-win10
This repo contains a Windows 10 UWP application that allows a user to traverse their organization structure as defined in Office 365/Azure AD. It queries Office 365/Azure AD using the Unified API (https://graph.microsoft.com).

![Windows 10 Org Explorer](http://i.imgur.com/697pQfi.png)

## Environment Setup and Prerequisites ##
Office 365 applications are secured by Azure Active Directory, which comes as part of an Office 365 subscription. If you do not have an Office 365 subscription or have it associated with Azure AD, then you should follow the steps to [Set up your Office 365 development environment](https://msdn.microsoft.com/office/office365/HowTo/setup-development-environment "Set up your Office 365 development environment") from MSDN.

Additionally, the application traverses an organization structure, so it is helpful to have users in your directory with reporting structure. Reporting structure is driven by a manager field on the user object. If you are using this application in a development environment, I recommend provisioning some users and creating a reporting structure (tip: you can provision users in Azure AD without assigning them Office 365 licenses).