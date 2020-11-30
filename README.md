# Kyameru.Component.File
Kyameru File Component works with Kyameru.Core and provides both a From and To route.

The File component can watch a directory for files and raise a message through a route to indicate it has found a new file. It can also move files from one place to another either by writing the contents of the Routable message OR from disk.

#### From

The from component is a simple system file watcher raising notifications for when a file has been:

* Added
* Modified
* Renamed

##### Setup Headers

Header | Description | Optional
------ | ----------- | --------
Target | Folder to watch | NO
Notifications | Type of notification to raise | NO
Filter | File watch filter | NO
SubDirectories | Include sub directories | YES

*Example Syntax*
```
Kyameru.Route.From("file:///C:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
```

##### Message Headers Raised
Header | Description
------ | -----------
SourceDirectory | Directory the event is raised from
SourceFile | File name of file picked up
FullSource | Full path of the file picked up
DateCreated | Date and time of the file (UTC)
Readonly | Boolean as to whether the file is readonly
Method | How the file was picked up
DataType | The data type of the body

#### To

The to component does a couple of very simple actions:

* Moves the picked up source file
* Copies the picked up source file
* Deletes the picked up source file
* Writes the contents of the body of the message to a file with the same name in the destination directory

##### Setup Headers

Header | Description | Optional
------ | ----------- | --------
Target | Destination Directory | NO
Action | Action To Perform | NO
Overwrite | Overwrites the destination if it exists | YES
