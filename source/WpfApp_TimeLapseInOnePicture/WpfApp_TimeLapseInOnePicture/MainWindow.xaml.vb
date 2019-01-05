Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms
Imports System.Windows.Threading

Class MainWindow
    Private Dico_ListFiles As New Dictionary(Of String, List(Of String))
    Private PathInput As String
    Private Sub Button_GetPath_Click(sender As Object, e As RoutedEventArgs) Handles Button_GetPath.Click
        Dim dialog = New FolderBrowserDialog()
        If dialog.ShowDialog() = Forms.DialogResult.OK Then
            TextBox_PathInput.IsReadOnly = False
            TextBox_PathInput.Text = dialog.SelectedPath
            TextBox_PathInput.IsReadOnly = True

            PathInput = TextBox_PathInput.Text

            Dim task = New Thread(AddressOf Load_Picture)

            task.Start()
            Do

            Loop While task.IsAlive()

            ListBox_Choose_Ext.Items.Clear()

            For Each ext In Dico_ListFiles.Keys
                ListBox_Choose_Ext.Items.Add(ext + "|" + Dico_ListFiles.Item(ext).Count.ToString)
            Next




            RadioButton_Ali_H.IsEnabled = True
            RadioButton_Ali_Random.IsEnabled = True
            RadioButton_Ali_V.IsEnabled = True
            RadioButton_Sort_AZ.IsEnabled = True
            RadioButton_Sort_Random.IsEnabled = True
            RadioButton_Sort_ZA.IsEnabled = True
        End If
    End Sub

    Private Sub Load_Picture()



        Log_Apllication("Start Analyse de " + PathInput)
        Dim strFileSize As String = ""
        Dim di As New IO.DirectoryInfo(PathInput)
        Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
        Dim fi As IO.FileInfo

        Dico_ListFiles = New Dictionary(Of String, List(Of String))
        For Each fi In aryFi
            strFileSize = (Math.Round(fi.Length / 1024)).ToString()
            Log_Apllication("************************")
            Log_Apllication("File Name: " + fi.Name)
            Log_Apllication("File Full Name: " + fi.FullName)
            Log_Apllication("File Size (KB): " + strFileSize)
            Log_Apllication("File Extension: " + fi.Extension)

            If IsValidImage(fi.FullName) Then
                If Not Dico_ListFiles.ContainsKey(fi.Extension) Then
                    Dico_ListFiles.Add(fi.Extension, New List(Of String))
                End If
                Dico_ListFiles.Item(fi.Extension).Add(fi.FullName)
            End If
        Next

        Log_Apllication("End Analyse de " + PathInput)
    End Sub
    Private Function IsValidImage(filename As String) As Boolean
        Try
            Dim img As System.Drawing.Image = System.Drawing.Image.FromFile(filename)
            img.Dispose()  ' Removes file-lock of IIS
        Catch generatedExceptionName As OutOfMemoryException
            ' Image.FromFile throws an OutOfMemoryException  
            ' if the file does not have a valid image format or 
            ' GDI+ does not support the pixel format of the file. 
            ' 
            Return False
        End Try
        Return True
    End Function
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        RadioButton_Ali_H.IsEnabled = False
        RadioButton_Ali_Random.IsEnabled = False
        RadioButton_Ali_V.IsEnabled = False
        RadioButton_Sort_AZ.IsEnabled = False
        RadioButton_Sort_Random.IsEnabled = False
        RadioButton_Sort_ZA.IsEnabled = False

        Button_Go.IsEnabled = False

        TextBox_PathInput.IsReadOnly = True

        RichTextBox_Log_WPF.IsReadOnly = True

    End Sub

    Private Sub Log_Apllication(txt As String)
        'Me.Dispatcher.BeginInvoke(New Action(Sub()

        Dim DateStr As String = Now.ToString("yyyy/MM/dd HH:mm:ss.ffff : ")
                                                 RichTextBox_Log_WPF.IsReadOnly = False
                                                 RichTextBox_Log_WPF.AppendText(DateStr + txt + vbCr)
                                                 RichTextBox_Log_WPF.IsReadOnly = True
                                                 RichTextBox_Log_WPF.ScrollToEnd()
        '   End Sub), DispatcherPriority.Normal)
    End Sub

    Private Sub ListBox_Choose_Ext_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListBox_Choose_Ext.SelectionChanged
        If ListBox_Choose_Ext.SelectedItem IsNot Nothing Then
            Button_Go.IsEnabled = True
        Else
            Button_Go.IsEnabled = False
        End If
    End Sub

    Private Sub Button_Go_Click(sender As Object, e As RoutedEventArgs) Handles Button_Go.Click
        Button_Go.IsEnabled = False
        Dim random As New Random()

        Dim Alignement As Alignement = Alignement.none
        If RadioButton_Ali_Random.IsChecked Then
            If random.Next(0, 1) = 0 Then
                Alignement = Alignement.H
            Else
                Alignement = Alignement.V
            End If
        ElseIf RadioButton_Ali_H.IsChecked Then
            Alignement = Alignement.H
        ElseIf RadioButton_Ali_V.IsChecked Then
            Alignement = Alignement.V
        Else
            'BUG
        End If

        Dim Sense As Sort = Sort.none
        If RadioButton_Sort_Random.IsChecked Then
            Sense = Sort.random
        ElseIf RadioButton_Sort_AZ.IsChecked Then
            Sense = Sort.a2z
        ElseIf RadioButton_Sort_ZA.IsChecked Then
            Sense = Sort.z2a
        Else
            'BUG
        End If

        Dim KeyExt = ListBox_Choose_Ext.SelectedItem.ToString.Split("|"c)(0)
        Dim listeFiles As List(Of String) = Dico_ListFiles.Item(KeyExt)
        listeFiles.Sort()

        Dim ListeFiles_Sorted As New List(Of String)
        Dim ListeFiles_no As New List(Of Integer)
        For i As Integer = 0 To listeFiles.Count - 1
            ListeFiles_no.Add(i)
        Next

        If Sense = Sort.random Then

            Do
                Dim NoSelected = random.Next(0, ListeFiles_no.Count - 1)
                If ListeFiles_no.Count = 1 Then NoSelected = 0
                ListeFiles_Sorted.Add(listeFiles.Item(ListeFiles_no.Item(NoSelected)))
                ListeFiles_no.RemoveAt(NoSelected)
            Loop While ListeFiles_no.Count > 0
        ElseIf Sense = Sort.a2z Then
            ListeFiles_Sorted = listeFiles
        ElseIf Sense = Sort.z2a Then
            Do
                Dim NoSelected = ListeFiles_no.Count - 1
                If ListeFiles_no.Count = 1 Then NoSelected = 0
                ListeFiles_Sorted.Add(listeFiles.Item(ListeFiles_no.Item(NoSelected)))
                ListeFiles_no.RemoveAt(NoSelected)
            Loop While ListeFiles_no.Count > 0
        End If

        'Get Size Tableau

        Dim imgDef As System.Drawing.Image = System.Drawing.Image.FromFile(ListeFiles_Sorted.Item(0))
        If Alignement = Alignement.V Then
            imgDef.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone)
        End If

        Dim Width As Integer = imgDef.Width
        Dim Height As Integer = imgDef.Height

        imgDef.Dispose()

        Dim Pas As Double = CDbl(Width) / CDbl(ListeFiles_Sorted.Count)


        'Dim data As New DataTable

        Dim data(Width, Height) As Class_RGB
        For w As Integer = 0 To Width - 1
            For h As Integer = 0 To Height - 1
                data(w, h) = New Class_RGB
            Next
        Next
        'For i As Integer = 0 To Width - 1
        '    data.Columns.Add(i.ToString)

        'Next

        'For j As Integer = 0 To Height - 1
        '    Dim listeInit As New List(Of Class_List_RGB)
        '    For i As Integer = 0 To Width - 1
        '        listeInit.Add(New Class_List_RGB)
        '    Next
        '    data.Rows.Add(listeInit.ToArray)
        'Next

        Me.Dispatcher.BeginInvoke(New Action(Sub()
                                                 For k As Integer = 0 To ListeFiles_Sorted.Count - 1
                                                     Dim PathFile As String = ListeFiles_Sorted.Item(k)
                                                     Log_Apllication("Analyse de " + PathFile + "(" + (k + 1).ToString + " sur " + ListeFiles_Sorted.Count.ToString + ")")

                                                     Dim Offcet As Double = Pas / 2

                                                     Dim Pas_X = Pas

                                                     Dim start As Integer = CInt((k - 1) * Pas_X + Offcet)
                                                     Dim fin As Integer = CInt((k + 1) * Pas_X + Offcet)

                                                     Dim Signal As New List(Of Double)
                                                     For m As Integer = 0 To fin - start + 2
                                                         Signal.Add(1 - Math.Cos(m * (2 * Math.PI) / (fin - start)))
                                                     Next


                                                     If k = 0 Then
                                                         start = 0
                                                         For n As Integer = 0 To CInt(Signal.Count / 2)
                                                             Signal.Item(n) = 2.0
                                                         Next
                                                     End If
                                                     If k = ListeFiles_Sorted.Count - 1 Then
                                                         fin = Width - 1

                                                         For n As Integer = CInt(Signal.Count / 2) To Signal.Count - 1
                                                             Signal.Item(n) = 2.0
                                                         Next
                                                     End If

                                                     'Get Value Pixel
                                                     Dim myBitmap = New Bitmap(ListeFiles_Sorted.Item(k))

                                                     For w As Integer = start To fin
                                                         For h As Integer = 0 To Height - 1
                                                             Dim Pixel As New Color
                                                             If Alignement = Alignement.V Then
                                                                 Pixel = myBitmap.GetPixel(h, w)
                                                             Else
                                                                 Pixel = myBitmap.GetPixel(w, h)
                                                             End If
                                                             data(w, h).Add(Pixel, Signal.Item(w - start))

                                                         Next
                                                     Next
                                                     myBitmap.Dispose()

                                                     Do
                                                         GC.Collect()
                                                     Loop Until GC.WaitForFullGCComplete = GCNotificationStatus.Succeeded Or GC.WaitForFullGCComplete = GCNotificationStatus.NotApplicable
                                                 Next

                                                 Dim myBitmapFinal = New Bitmap(Width, Height)
                                                 For w As Integer = 0 To Width - 1
                                                     For h As Integer = 0 To Height - 1

                                                         Dim Rh_RGB As Class_RGB = data(w, h).Get_Value
                                                         Dim Pixel As Color = Color.FromArgb(Math.Min(Rh_RGB.R, 254), Math.Min(Rh_RGB.G, 254), Math.Min(Rh_RGB.B, 254))

                                                         myBitmapFinal.SetPixel(w, h, Pixel)

                                                     Next
                                                 Next
                                                 myBitmapFinal.Save("C:\temp\final.jpg")
                                             End Sub), DispatcherPriority.Background)

        ' GeneratePicture(ListeFiles_Sorted, Alignement)
        Button_Go.IsEnabled = True

    End Sub

    'Private Function GetPlage(id As Integer, nb As Integer, pas As Double) As Return_Plage
    '    Dim a = New Return_Plage
    '    Dim Prefix As Integer = CInt(pas / 2)
    '    Dim Suffix As Integer = CInt(pas / 2)
    '    If id = 0 Then
    '        Prefix = 0
    '    End If
    '    If id = nb - 1 Then
    '        Suffix = 0
    '    End If

    '    Dim start As Integer = CInt(id * pas)
    '    Dim fin As Integer = CInt((id + 1) * pas)

    '    a.Min = start - Prefix
    '    a.Max = fin + Suffix

    '    Return a
    'End Function




    'Private Sub GeneratePicture(ListeFiles_Sorted As List(Of String), Alignement As Alignement)
    '    Dim imgDef As System.Drawing.Image = System.Drawing.Image.FromFile(ListeFiles_Sorted.Item(0))
    '    If Alignement = Alignement.V Then
    '        imgDef.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone)
    '    End If

    '    Dim Width As Integer = imgDef.Width
    '    Dim Height As Integer = imgDef.Height

    '    imgDef.Dispose()

    '    Dim Pas As Double = CDbl(Width) / CDbl(ListeFiles_Sorted.Count)



    '    Dim Arg_Threads As New Arg_Thread
    '    Arg_Threads.Pas = Pas
    '    Arg_Threads.Id = 0
    '    Arg_Threads.ListeFiles_Sorted = ListeFiles_Sorted
    '    Arg_Threads.Alignement = Alignement

    '    GetCrop(Arg_Threads)
    'End Sub

    'Private Function GetCrop(Arg_Threads As Arg_Thread) As Image








    'End Function



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

