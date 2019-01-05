Imports System.Drawing

Public Class Class_RGB
    Public Property R As Integer = 0
    Public Property G As Integer = 0
    Public Property B As Integer = 0
    Public Property Coef As Double = 0.0

    Public Sub New()

    End Sub

    Public Sub Add(Pixel As Color, Coef As Double)
        Me.R += CInt(Pixel.R * Coef)
        Me.G += CInt(Pixel.G * Coef)
        Me.B += CInt(Pixel.B * Coef)
        Me.Coef += Coef
    End Sub

    Public Function Get_Value() As Class_RGB
        Dim RGB As New Class_RGB
        RGB.R = CInt(Me.R / Me.Coef)
        RGB.G = CInt(Me.G / Me.Coef)
        RGB.B = CInt(Me.B / Me.Coef)
        RGB.Coef = Me.Coef / Me.Coef
        Return RGB
    End Function

End Class
