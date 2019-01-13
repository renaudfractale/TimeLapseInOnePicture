Public Class Class_Limite
    Public Property Start As Integer = 0
    Public Property Fin As Integer = 0
    Public Property Dif As Integer = 0
    Public Property H As Integer = 0
    Public Property PathFileTemp As String = ""
    Public Sub New(index As Integer, Conf As Class_Enum, CountFiles As Integer, Width As Integer, Height As Integer, PathTempCompute As String)

        Dim Pas = CDbl(Width) / CDbl(CountFiles)
        If Conf.Alignement = Alignement.V Then
            Pas = CDbl(Height) / CDbl(CountFiles)
        End If

        Dim Offcet As Double = Pas / 2

        Dim Pas_X = Pas

        Start = CInt((index - 1) * Pas_X + Offcet)
        Fin = CInt((index + 1) * Pas_X + Offcet) - 1


        If index = 0 Then
            Start = 0
        ElseIf index = CountFiles - 1 Then
            If Conf.Alignement = Alignement.V Then
                Fin = Height - 1
            Else
                Fin = Width - 1
            End If
        End If
        Dif = Fin - Start

        H = Height
        If Conf.Alignement = Alignement.V Then
            H = Width
        End If


        PathFileTemp = PathTempCompute + "\" + index.ToString + ".png"

    End Sub
End Class
