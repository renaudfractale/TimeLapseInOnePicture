Imports System.IO
Imports System.Reflection

Class Application

    ' Les événements de niveau application, par exemple Startup, Exit et DispatcherUnhandledException
    ' peuvent être gérés dans ce fichier.
    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        Me.CopyDatabase()
    End Sub

    Private Sub CopyDatabase()
        'Dim roamingPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        'Dim location As String = Assembly.GetExecutingAssembly().Location
        'Dim index As Integer = location.LastIndexOf("\")
        'Dim sourcePath As String = location.Substring(0, index)
        'Dim destinationPath As String = Path.Combine(roamingPath, "MyApp")
        'Dim fileName As String = "Northwind.sdf"

        'If Not Directory.Exists(destinationPath) Then
        '    Directory.CreateDirectory(destinationPath)
        'End If

        'Dim sourceFile As String = Path.Combine(sourcePath, fileName)
        'Dim destFile As String = Path.Combine(destinationPath, fileName)
        'File.Copy(sourceFile, destFile, True)

    End Sub
End Class
