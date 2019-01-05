Public Class Class_Enum
    Public Property Sort As Sort = Sort.none
    Public Property Alignement As Alignement = Alignement.none
    Public Property Signal As Signal = Signal.none
    Public Property TextSelected As String = ""
    Public Sub New()

    End Sub
End Class



Public Enum Sort
    none
    random
    a2z
    z2a
End Enum


Public Enum Alignement
    none
    H
    V
End Enum

Public Enum Signal
    none
    Rect
    Cos
    Trig
End Enum
