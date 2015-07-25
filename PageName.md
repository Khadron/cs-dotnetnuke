#How to build a working copy from SVN

# Creating a build for development from SVN #

Step 1:

Check out the project from http://cs-dotnetnuke.googlecode.com/svn/trunk
Step 2:

Ensure that there is an IIS ASP .NET website called CS\_DNN\_DEV defined, point this to \CS\_Website within the directory that you checked out
Step 3:

Open the project (CS\_DotNetNuke.sln) in visual studio and build it
Step 4:

A blank database and user needs to be created for DotNetNuke...

i) 	Open SQL server management express (or a suitable sql server management tool, instructions here are for SQL Server Management Express 2005)
ii) 	Right click on the databases tab and add a new database, I have chosen 'DNCS' in this case, press 'OK', to create the database.
iii)	A user needs to be created for the database, expand the security tab, right click on 'Logins' and create a new login

> Login name 'DNCS'
> SQL Server Authentication
> > (enter a password and confirm it)...for testing purposes, I have chosen 'DNCS' as the password (to keep things simple)


> Uncheck 'Enforce Password Policy', 'Enforce Password Expiration', 'User must change password at next login'
> Default database [DNCS](DNCS.md)
> English

> Click on 'User Mapping'
> Click the checkbox next to DNCS
> Click the db\_owner checkbox

> Press 'OK'

Step 4:

> Adjust Web.Config to contain the correct connection string, typically this would be in a form similar to...

> server=WS028\SQLEXPRESS2;database=cs\_dnn\_dev;User ID=DNCS;Password=DNCS;Initial Catalog=DNCS

> The connection string is defined in two active confuguration areas, be sure to update them both (both can accept the same style of connection string)

Step 5:

> Visit http://localhost/CS_DNN_DEV/install/install.aspx to start the installer

> The installation should complete and you will be given a link to the main portal page...



# Details #

Add your content here.  Format your content with:
  * Text in **bold** or _italic_
  * Headings, paragraphs, and lists
  * Automatic links to other wiki pages