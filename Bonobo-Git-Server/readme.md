Bonobo Git Server
==============================================

[![Build status](https://ci.appveyor.com/api/projects/status/4vyllwtb5i645lrt/branch/master?svg=true)](https://ci.appveyor.com/project/jakubgarfield/bonobo-git-server)

Thank you for downloading Bonobo Git Server. For more information please visit [http://bonobogitserver.com](http://bonobogitserver.com).


Prerequisites
-----------------------------------------------

* Internet Information Services 7 and higher
    * [How to Install IIS 8 on Windows 8](http://www.howtogeek.com/112455/how-to-install-iis-8-on-windows-8/)
    * [Installing IIS 8 on Windows Server 2012](http://www.iis.net/learn/get-started/whats-new-in-iis-8/installing-iis-8-on-windows-server-2012)
    * [Installing IIS 7 on Windows Server 2008 or Windows Server 2008 R2](http://www.iis.net/learn/install/installing-iis-7/installing-iis-7-and-above-on-windows-server-2008-or-windows-server-2008-r2)
    * [Installing IIS 7 on Windows Vista and Windows 7](http://www.iis.net/learn/install/installing-iis-7/installing-iis-on-windows-vista-and-windows-7)
* ASP.NET MVC 4
* .NET Framework 4.6

Links
-----------------------------------------------

* [Web page](http://bonobogitserver.com/)
* [Documentation](http://bonobogitserver.com/documentation/)
* [Changelog](https://github.com/jakubgarfield/Bonobo-Git-Server/blob/master/changelog.md)
* [License](https://github.com/jakubgarfield/Bonobo-Git-Server/blob/master/license.md)

Build
-----------------------------------------------

Ensure you have downloaded the Git tools using the [get-git.ps1](get-git.ps1) script or by letting Visual Studio restore the nuget packages.
To do it manually, run `msbuild get-git.msbuild`.

You can now build it in Visual Studio or by running `msbuild Bonobo.Git.Server.sln`.

Features
-----------------------------------------------

* Git Server
    * manage users
    * manage teams
    * manage repositories
    * anonymous clone, pull, push
* Repository Browser
    * commit history
    * repository tree
    * blob detail
    * commit detail
    * blame view
    * zipped repository download
* Active Directory integration
* Translated into several languages

## 🎨 Custom UI Modernization (Corporate Edition)
-----------------------------------------------
This specific repository includes a highly customized, modernized frontend tailored for secure, air-gapped corporate environments (e.g., VPNs with strict firewall rules and no external internet access).

**Key Frontend Upgrades:**
* **Glassmorphism Dark Mode:** A sleek, frosted-glass UI redesign applied to the master layout, login, and dashboards.
* **Dynamic Local Wallpapers:** Integration of dynamic Bing backgrounds with a dark readability overlay ensuring high contrast.
* **100% Local Assets (Zero CDNs):** All frontend dependencies are hosted locally within `~/Librerias/` to comply with strict banking security policies.
* **Modern Tooling:** Upgraded with Bootstrap 5, FontAwesome 6, DataTables, SweetAlert2, and Toastr.
* **AI-Assisted Development:** Features an `AI_NOTES.md` strategic manual at the root level to guide LLM agents (like Jules, Cursor, or Copilot) in maintaining ASP.NET MVC Razor syntax rules and strict UI constraints without breaking the backend.

IIS Deployment
-----------------------------------------------

1. Extract the release zip file or build the source code
2. Put the `Bonobo.Git.Server` folder to `C:\inetpub\wwwroot`
3. Convert `Bonobo.Git.Server` folder to an application in IIS
4. Make sure that the `App_Data` folder is writable for IIS user

For more detailed information, please read the [install guide](http://bonobogitserver.com/install/).

Environment Variables
-----------------------------------------------

Want to add some git hooks? E.g. to automatically run CI on push? But want to know who is the web frontend usernam?

Bonobo provides the following environment variables:

* `AUTH_USER`: The username used to login. Empty if it was an anonymous operation (clone/push/pull)
* `REMOTE_USER`: Same as `AUTH_USER`
* `AUTH_USER_TEAMS`: A comma-separated list containing all the teams the user belongs to. Commas in teams name are escaped with a backslash. Backslashes are also escaped with a `\`. Example: Teams 'Editors\ Architects', 'Programmers,Testers' will become `Editors\\ Architects,Programmers\,Testers`.
* `AUTH_USER_ROLES`: A comma-separated list containing all the roles the user belongs to. Commas in roles are escaped with a backslash. Backslashes are also escaped with a `\`.
* `AUTH_USER_DISPLAYNAME`: Given Name + Surname if available. Else the username.

**Beware that due to the way HTTP basic authentication works, if anonymous operations (push/pull) are enabled the variables above will always be empty!**

New release
-----------------------------------------------

* update [changelog](https://github.com/jakubgarfield/Bonobo-Git-Server/blob/master/changelog.md)
* update version numbers in [appveyor.yml](https://github.com/jakubgarfield/Bonobo-Git-Server/blob/master/appveyor.yml)
* add tag so it appears under [releases](https://github.com/jakubgarfield/Bonobo-Git-Server/releases) with `git tag -a 6.0.0 -m "Release ..."`