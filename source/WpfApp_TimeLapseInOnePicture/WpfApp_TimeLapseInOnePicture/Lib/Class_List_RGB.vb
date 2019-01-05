Public Class Class_List_RGB
    Inherits List(Of Class_RGB)

    Public Function Get_ValueMoyen() As Class_RGB
        Dim RGB As New Class_RGB

        For Each value As Class_RGB In MyBase.ToList
            Dim RGB_temp As Class_RGB = value.Get_Value()
            RGB.R += RGB_temp.R
            RGB.G += RGB_temp.G
            RGB.B += RGB_temp.B
            RGB.Coef += RGB_temp.Coef
        Next

        If RGB.Coef = 0.0 Then
            RGB.R = CInt(RGB.R / RGB.Coef)

            RGB.B = CInt(RGB.B / RGB.Coef)

            RGB.G = CInt(RGB.G / RGB.Coef)
        End If
        RGB.Coef = 1.0

        Return RGB
    End Function
End Class
