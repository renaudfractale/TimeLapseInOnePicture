Public Class Class_index_files
    Inherits List(Of Class_index_file)
    Public Property PathMain As String = ""
    Public Property Conf As New Class_Enum
    Public Sub New(PathMain As String)
        Me.PathMain = PathMain
    End Sub
End Class

Public Class Class_index_file
    Public Property Id As Integer = 0
    Public Property IsUsed As Boolean = False
    Public Property PathFile As String = ""

    Public Sub New(no As Integer, PathFile As String)
        Me.Id = no
        Me.PathFile = PathFile
    End Sub

End Class
