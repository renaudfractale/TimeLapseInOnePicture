Imports System.Windows.Forms

Class MainWindow
    Private m_Runtime As Class_Runtime

    Private Sub Set_StateButtonGo(State As Boolean)
        Button_Go.IsEnabled = State
        Button_Go_All.IsEnabled = State
    End Sub
    Private Sub Button_GetPath_Click(sender As Object, e As RoutedEventArgs) Handles Button_GetPath.Click
        Dim dialog = New FolderBrowserDialog()
        If dialog.ShowDialog() = Forms.DialogResult.OK Then
            TextBox_PathInput.IsReadOnly = False
            TextBox_PathInput.Text = dialog.SelectedPath
            TextBox_PathInput.IsReadOnly = True

            Cursor = System.Windows.Input.Cursors.Wait()
            Dim liste = m_Runtime.AnalysePath(dialog.SelectedPath)

            ListBox_Choose_Ext.Items.Clear()

            For Each item In liste
                ListBox_Choose_Ext.Items.Add(item)
            Next

            RadioButton_Ali_H.IsEnabled = True
            RadioButton_Ali_Random.IsEnabled = True
            RadioButton_Ali_V.IsEnabled = True

            RadioButton_Sort_AZ.IsEnabled = True
            RadioButton_Sort_Random.IsEnabled = True
            RadioButton_Sort_ZA.IsEnabled = True

            RadioButton_Signal_Cos.IsEnabled = True
            RadioButton_Signal_Rect.IsEnabled = True
            RadioButton_Signal_Trig.IsEnabled = True

            Cursor = System.Windows.Input.Cursors.Arrow
        End If
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        RadioButton_Ali_H.IsEnabled = False
        RadioButton_Ali_Random.IsEnabled = False
        RadioButton_Ali_V.IsEnabled = False

        RadioButton_Sort_AZ.IsEnabled = False
        RadioButton_Sort_Random.IsEnabled = False
        RadioButton_Sort_ZA.IsEnabled = False

        RadioButton_Signal_Cos.IsEnabled = False
        RadioButton_Signal_Rect.IsEnabled = False
        RadioButton_Signal_Trig.IsEnabled = False

        Set_StateButtonGo(False)


        TextBox_PathInput.IsReadOnly = True

        RichTextBox_Log_WPF.IsReadOnly = True

        m_Runtime = New Class_Runtime(Me)

    End Sub


    Private Sub ListBox_Choose_Ext_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ListBox_Choose_Ext.SelectionChanged
        If ListBox_Choose_Ext.SelectedItem IsNot Nothing Then
            Set_StateButtonGo(True)
        Else
            Set_StateButtonGo(False)
        End If
    End Sub

    Private Sub Button_Go_Click(sender As Object, e As RoutedEventArgs) Handles Button_Go.Click
        Set_StateButtonGo(False)
        Dim random As New Random()

        Dim Enum_Radio As New Class_Enum
        If RadioButton_Ali_Random.IsChecked Then
            If random.Next(0, 1) = 0 Then
                Enum_Radio.Alignement = Alignement.H
            Else
                Enum_Radio.Alignement = Alignement.V
            End If
        ElseIf RadioButton_Ali_H.IsChecked Then
            Enum_Radio.Alignement = Alignement.H
        ElseIf RadioButton_Ali_V.IsChecked Then
            Enum_Radio.Alignement = Alignement.V
        Else
            Throw New System.Exception("Alignement Error in MainWindow")
        End If

        If RadioButton_Sort_Random.IsChecked Then
            Enum_Radio.Sort = Sort.random
        ElseIf RadioButton_Sort_AZ.IsChecked Then
            Enum_Radio.Sort = Sort.a2z
        ElseIf RadioButton_Sort_ZA.IsChecked Then
            Enum_Radio.Sort = Sort.z2a
        Else
            Throw New System.Exception("Sort Error in MainWindow")
        End If

        If RadioButton_Signal_Cos.IsChecked Then
            Enum_Radio.Signal = Signal.Cos
        ElseIf RadioButton_Signal_Trig.IsChecked Then
            Enum_Radio.Signal = Signal.Trig
        ElseIf RadioButton_Signal_Rect.IsChecked Then
            Enum_Radio.Signal = Signal.Rect
        Else
            Throw New System.Exception("Signal Error in MainWindow")
        End If

        Enum_Radio.TextSelected = CType(ListBox_Choose_Ext.SelectedItem, String)

        Dim PathPicture = m_Runtime.GeneratePicture(Enum_Radio)

        Dim startInfo = New ProcessStartInfo(PathPicture)
        Process.Start(startInfo)

        Set_StateButtonGo(True)
    End Sub

    Private Sub Button_Go_All_Click(sender As Object, e As RoutedEventArgs) Handles Button_Go_All.Click
        Set_StateButtonGo(False)

        Dim Enum_Radio As New Class_Enum
        Enum_Radio.TextSelected = CType(ListBox_Choose_Ext.SelectedItem, String)

        For Ali As Integer = 0 To 1
            If Ali = 0 Then
                Enum_Radio.Alignement = Alignement.H
            Else
                Enum_Radio.Alignement = Alignement.V
            End If
            For Sens As Integer = 0 To 2
                If Sens = 0 Then
                    Enum_Radio.Sort = Sort.random
                ElseIf Sens = 1 Then
                    Enum_Radio.Sort = Sort.a2z
                Else
                    Enum_Radio.Sort = Sort.z2a
                End If
                For Signale As Integer = 0 To 2
                    If Signale = 0 Then
                        Enum_Radio.Signal = Signal.Rect
                    ElseIf Signale = 1 Then
                        Enum_Radio.Signal = Signal.Trig
                    Else
                        Enum_Radio.Signal = Signal.Cos
                    End If
                    Dim PathPicture = m_Runtime.GeneratePicture(Enum_Radio)
                    Dim startInfo = New ProcessStartInfo(PathPicture)
                    Process.Start(startInfo)
                Next
            Next
        Next
        Set_StateButtonGo(True)
    End Sub

    Private Sub Window_Closed(sender As Object, e As EventArgs)
        m_Runtime.Clear_Temp()
    End Sub
End Class
