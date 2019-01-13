Public Class Class_index_files
    Inherits List(Of Class_index_file)


End Class

Public Class Class_index_file
    Public Property Id As Integer = 0
    Public Property IsUsed As Boolean = False
    Public Property PathFile As String = ""
    Public Property PathMain As String = ""
    Public Property Conf As New Class_Enum
    Public Property Count As Integer = 0
    Public Sub New(no As Integer, PathFile As String, PathMain As String, Conf As Class_Enum, Count As Integer)
        Me.Id = no
        Me.PathFile = PathFile
        Me.Conf = Conf
        Me.PathMain = PathMain
        Me.Count = Count
    End Sub

End Class
