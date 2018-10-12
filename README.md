# Umbraco media cleaner (WIP)
A console app that searches for Umbraco Orphaned Media and moves it out of the public media folder. 

If left unattended, media recycled via the Umbraco back office will still be present in the media folder. Search engines like Google will still index these media items as they are still present in the media folder.

## How to use

Before running this program you must add your settings to the config. There are multiple ways to do this either add your settings into the **app.secrets.config** and when you **build** the solution it will be merged into **Media-Cleaner.exe.config** in the bin folder. Or just build the project and add your changes within the **Media-Cleaner.exe.config** after building.

### Guide to the config

The **connection string** is your **Umbraco Database** connection string.

`<add key="ConnectionString" value="secret-string"/>`

The **media location** will be the **web root** or **parent folder** that contains the **media** folder. The app looks for a folder called media. (This is something that I will change)

`<add key="MediaLocation" value="E:\webroot"/>`

**Drop Off  Location** is where you want to **move** the orphaned media. There are **no delete operations** performed in this app, its basically cutting and pasting the media.

`<add key="DropOffLocation" value="E:\dropOff"/>`

**Perform Move**, if set to **true** will **move** the **media** when the app is ran. By **default** it is set to **false** this way the app only **reports** what it finds and doesn't move anything.

`<add key="PerformMove" value="false"/>`

**Show Bad Files Only** is a personal preference the app will create a log in the logs folder for each time your run it. Setting Show Bad Files Only to **false** will make the **log include all files** its found good and bad which can get quite long depending how much media you have.

`<add key="ShowBadFilesOnly" value="true"/>`

As mentioned a log is generated every time you run the application.

With all operations that involve moving things, I recommend **backing up your media** just in case!

Once you have added your settings 

## Bugs to fix / Features to add

Allow user to specify direct path to media folder

Allow user to specify a url instead of a path