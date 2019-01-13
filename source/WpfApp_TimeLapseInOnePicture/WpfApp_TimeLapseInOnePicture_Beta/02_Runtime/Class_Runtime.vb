Imports System.Drawing
Imports System.Threading
Imports System.Windows.Threading

Class Class_Runtime
    Private m_nbThread As Integer = 18
    Private m_MainWindow As MainWindow
    Private m_PathInput As String
    'Private m_Index_FilesInputes As Class_index_files
    Private m_Dico_ListFiles As Dictionary(Of String, Dictionary(Of String, List(Of String)))
    Private PathTempRoot As String = ""
    Private PathTempCompute As String = ""
    Public Sub New(MainWindow As MainWindow)
        m_MainWindow = MainWindow
        'WriteLog("Start -->  Del Path Temp")
        Me.PathTempRoot = System.IO.Path.GetTempPath() + "TimeLapseInOnePicture_" + Now.ToString("YYYY-MM-dd_HH-mm-ss_fffff")
        Me.PathTempCompute = PathTempRoot + "\Compute"
        Rmdir(Me.PathTempRoot)
        Mkdir(Me.PathTempRoot)
    End Sub
    Public Sub Clear_Temp()
        Rmdir(Me.PathTempRoot)
    End Sub
    Private Sub Rmdir(PathDir As String)
        Dim myProcess1 As New Process()
        myProcess1.StartInfo.FileName = "cmd.exe"  'l'application
        myProcess1.StartInfo.Arguments = "/c rmdir /Q /S " + PathDir  'les paramètres passés à l'application
        myProcess1.Start()  'lance le process
        myProcess1.WaitForExit()  'attend qu'il soit terminé avant d'aller plus loin
        myProcess1.Close()  'ferme le process
    End Sub
    Private Sub Mkdir(PathDir As String)
        Dim myProcess2 As New Process()
        myProcess2.StartInfo.FileName = "cmd.exe"  'l'application
        myProcess2.StartInfo.Arguments = "/c mkdir " + PathDir  'les paramètres passés à l'application
        myProcess2.Start()  'lance le process
        myProcess2.WaitForExit()  'attend qu'il soit terminé avant d'aller plus loin
        myProcess2.Close()  'ferme le process
    End Sub
    Public Function AnalysePath(PathInput As String) As List(Of String)
        m_PathInput = PathInput
        Log_Apllication("Start Analyse de " + PathInput)

        Dim di As New IO.DirectoryInfo(PathInput)
        Dim ListOf_FilesInputes = di.GetFiles("*.*").ToList

        Dim index_FilesInputes = New Class_index_files()
        'Création de l'indxe
        For i As Integer = 0 To ListOf_FilesInputes.Count - 1
            index_FilesInputes.Add(New Class_index_file(i, ListOf_FilesInputes.Item(i).FullName, "", New Class_Enum, ListOf_FilesInputes.Count))
        Next

        'Init dico
        m_Dico_ListFiles = New Dictionary(Of String, Dictionary(Of String, List(Of String)))

        Parallel.ForEach(index_FilesInputes,
                     Sub(FilesInpute)
                         Analyse_Files(FilesInpute)
                     End Sub)


        'Dim listThread As New List(Of Thread)
        'For i As Integer = 1 To m_nbThread
        '    listThread.Add(New Thread(AddressOf Analyse_Files))
        '    listThread.Last.Start()
        'Next


        'For i As Integer = 0 To m_nbThread - 1
        '    listThread.Item(i).Join()
        'Next

        Dim ListeBox As New List(Of String)

        For Each Ext In m_Dico_ListFiles.Keys
            Dim dico = m_Dico_ListFiles.Item(Ext)
            For Each Size In dico.Keys
                Dim count = dico.Item(Size).Count
                ListeBox.Add(Ext + "|" + Size + "|" + count.ToString)
            Next
        Next
        Log_Apllication("End Analyse de " + PathInput)

        Return ListeBox
    End Function

    Private Sub Analyse_Files(FilesInpute As Class_index_file)


        Dim PathFile = FilesInpute.PathFile
        Dim fi = New IO.FileInfo(PathFile)
                Dim strFileSize = (Math.Round(fi.Length / 1024)).ToString()
                Log_Apllication("************************")
                Log_Apllication("File Name: " + fi.Name)
                Log_Apllication("File Full Name: " + fi.FullName)
                Log_Apllication("File Size (KB): " + strFileSize)
                Log_Apllication("File Extension: " + fi.Extension)
                Dim txt = IsValidImage(fi.FullName)
                If txt <> "" Then
            SyncLock m_Dico_ListFiles
                'si extention inconue
                If Not m_Dico_ListFiles.ContainsKey(fi.Extension) Then
                    m_Dico_ListFiles.Add(fi.Extension, New Dictionary(Of String, List(Of String)))
                End If

                'Si dimenstion picture inconnu
                If Not m_Dico_ListFiles.Item(fi.Extension).ContainsKey(txt) Then
                    m_Dico_ListFiles.Item(fi.Extension).Add(txt, New List(Of String))
                End If

                'Ajout du fichier
                m_Dico_ListFiles.Item(fi.Extension).Item(txt).Add(fi.FullName)
            End SyncLock
        End If



    End Sub
    Private Function IsValidImage(filename As String) As String
        Dim txt As String = ""
        Try
            Dim img As System.Drawing.Image = System.Drawing.Image.FromFile(filename)
            Dim w As Integer = img.Width
            Dim h As Integer = img.Height

            txt = w.ToString + "x" + h.ToString
            img.Dispose()  ' Removes file-lock of IIS



        Catch generatedExceptionName As OutOfMemoryException
            ' Image.FromFile throws an OutOfMemoryException  
            ' if the file does not have a valid image format or 
            ' GDI+ does not support the pixel format of the file. 
            ' 

        End Try
        Return txt
    End Function

    Private Sub Log_Apllication(txt As String)
        m_MainWindow.Dispatcher.BeginInvoke(New Action(Sub()

                                                           Dim DateStr As String = Now.ToString("yyyy/MM/dd HH:mm:ss.ffff : ")
                                                           m_MainWindow.RichTextBox_Log_WPF.IsReadOnly = False
                                                           m_MainWindow.RichTextBox_Log_WPF.AppendText(DateStr + txt + vbCr)
                                                           m_MainWindow.RichTextBox_Log_WPF.IsReadOnly = True
                                                           m_MainWindow.RichTextBox_Log_WPF.ScrollToEnd()
                                                       End Sub), DispatcherPriority.Background)
    End Sub


    Public Function GeneratePicture(InputeParameter As Class_Enum) As String
        ' /////////////////// Genration du fichier de sortie \\\\\\\\\\\\\\\\\\\\
        Dim PathOutpute As String = m_PathInput + "\" + "TimeLapseInOnePicture"
        If Not My.Computer.FileSystem.DirectoryExists(PathOutpute) Then
            My.Computer.FileSystem.CreateDirectory(PathOutpute)
        End If
        PathOutpute += "\Output_"
        Dim ext As String = ".jpg"
        Dim NoFile As Integer = -1
        Do
            NoFile += 1
        Loop While My.Computer.FileSystem.FileExists(PathOutpute + NoFile.ToString + ext)
        PathOutpute += NoFile.ToString + ext

        ' /////////////////// Generation de l'ordre (Sort) \\\\\\\\\\\\\\\\\\\\
        Dim ExtInpute As String = InputeParameter.TextSelected.Split("|"c)(0)
        Dim SizeInpute As String = InputeParameter.TextSelected.Split("|"c)(1)
        Dim liste_Files As List(Of String) = m_Dico_ListFiles.Item(ExtInpute).Item(SizeInpute)
        liste_Files.Sort()

        'Creation de la liste d'indexe
        Dim Liste_Indexe As New List(Of Integer)
        For i As Integer = 0 To liste_Files.Count - 1
            Liste_Indexe.Add(i)
        Next

        'Gestion des options
        Dim Liste_FilesSorted As New List(Of String)
        If InputeParameter.Sort = Sort.a2z Then
            Liste_FilesSorted = liste_Files
        ElseIf InputeParameter.Sort = Sort.z2a Then
            Do
                Dim NoSelected = Liste_Indexe.Count - 1
                Liste_FilesSorted.Add(liste_Files.Item(Liste_Indexe.Item(NoSelected)))
                Liste_Indexe.RemoveAt(NoSelected)
            Loop While Liste_Indexe.Count > 0
        ElseIf InputeParameter.Sort = Sort.random Then
            Dim random As New Random()
            Do
                Dim NoSelected = random.Next(0, Liste_Indexe.Count - 1)
                Liste_FilesSorted.Add(liste_Files.Item(Liste_Indexe.Item(NoSelected)))
                Liste_Indexe.RemoveAt(NoSelected)
            Loop While Liste_Indexe.Count > 0
        Else
            Throw New System.Exception("Sort Error in RunTime")
        End If


        'Création de l'indxe de thread
        Dim Index_FilesInputes = New Class_index_files()
        For i As Integer = 0 To Liste_FilesSorted.Count - 1
            Index_FilesInputes.Add(New Class_index_file(i, Liste_FilesSorted(i), PathOutpute, InputeParameter, Liste_FilesSorted.Count))
        Next



        '/////////////// INIT Path \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        Rmdir(PathTempCompute)
        Mkdir(PathTempCompute)

        '//////////////////////// Thread \\\\\\\\\\\\\\\\\\\\\\\\\\\\

        Parallel.ForEach(Index_FilesInputes,
                     Sub(FilesInpute)
                         Compute_Files(FilesInpute)
                     End Sub)

        'Dim listThread As New List(Of Thread)
        'For i As Integer = 1 To m_nbThread
        '    listThread.Add(New Thread(AddressOf Compute_Files))
        '    listThread.Last.Start()
        'Next


        'For i As Integer = 0 To m_nbThread - 1
        '    listThread.Item(i).Join()
        'Next

        Log_Apllication("Start Asemblage de " + PathOutpute)

        Dim myBitmap = New System.Drawing.Bitmap(Liste_FilesSorted.Item(0))
        Dim Width As Integer = myBitmap.Width
        Dim Height As Integer = myBitmap.Height
        myBitmap.Dispose()



        Dim FinalJPG As Bitmap

        If InputeParameter.Alignement = Alignement.H Then
            FinalJPG = New Bitmap(Width, Height)
        Else
            FinalJPG = New Bitmap(Height, Width)
        End If

        Log_Apllication("Start INIT de " + PathOutpute)
        For i As Integer = 0 To FinalJPG.Width - 1
            For j As Integer = 0 To FinalJPG.Height - 1
                FinalJPG.SetPixel(i, j, System.Drawing.Color.FromArgb(0, 0, 0, 0))
            Next
        Next
        Log_Apllication("End INIT de " + PathOutpute)

        For index As Integer = 0 To Liste_FilesSorted.Count - 1
            Dim Limite As New Class_Limite(index, InputeParameter, Liste_FilesSorted.Count, Width, Height, PathTempCompute)
            Dim myPNGTemp = New System.Drawing.Bitmap(Limite.PathFileTemp)

            For i As Integer = 0 To Limite.Dif - 1

                For j As Integer = 0 To Limite.H - 1
                    Dim PixelOld = FinalJPG.GetPixel(i + Limite.Start, j)
                    Dim PixelFrameTemp = myPNGTemp.GetPixel(i, j)
                    Dim PixelNew As New System.Drawing.Color
                    Dim A As Double = (CDbl(PixelOld.A) + CDbl(PixelFrameTemp.A)) / 127.0
                    Dim R As Double = (CDbl(PixelOld.R) * CDbl(PixelOld.A) / 127.0 + CDbl(PixelFrameTemp.R) * CDbl(PixelFrameTemp.A) / 127.0)
                    Dim G As Double = (CDbl(PixelOld.G) * CDbl(PixelOld.A) / 127.0 + CDbl(PixelFrameTemp.G) * CDbl(PixelFrameTemp.A) / 127.0)
                    Dim B As Double = (CDbl(PixelOld.B) * CDbl(PixelOld.A) / 127.0 + CDbl(PixelFrameTemp.B) * CDbl(PixelFrameTemp.A) / 127.0)
                    If A <> 0.0 Then
                        R = R / A
                        G = G / A
                        B = B / A
                    End If
                    PixelNew = System.Drawing.Color.FromArgb(CInt(A * 127), CInt(R), CInt(G), CInt(B))
                    FinalJPG.SetPixel(i + Limite.Start, j, PixelNew)
                Next

            Next
                myPNGTemp.Dispose()
        Next
        Log_Apllication("Start Clear Alpha de " + PathOutpute)
        For i As Integer = 0 To FinalJPG.Width - 1
            For j As Integer = 0 To FinalJPG.Height - 1
                Dim PixelOld = FinalJPG.GetPixel(i, j)
                Dim PixelNew As New System.Drawing.Color
                PixelNew = System.Drawing.Color.FromArgb(0, PixelOld.R, PixelOld.G, PixelOld.B)
                FinalJPG.SetPixel(i, j, PixelNew)
            Next
        Next
        Log_Apllication("End Clear Alpha " + PathOutpute)

        FinalJPG.Save(PathOutpute, Imaging.ImageFormat.Jpeg)
        FinalJPG.Dispose()
        If InputeParameter.Alignement = Alignement.V Then
            Log_Apllication("Start Flip File " + PathOutpute)
            Dim FilpFile As System.Drawing.Image = System.Drawing.Image.FromFile(PathOutpute)
            FilpFile.RotateFlip(RotateFlipType.Rotate90FlipX)
            FilpFile.Save(PathOutpute)
            FilpFile.Dispose()
            Log_Apllication("End Flip File " + PathOutpute)
        End If

        Log_Apllication("End Asemblage de " + PathOutpute)
        Return PathOutpute
    End Function



    Private Sub Compute_Files(FilesInpute As Class_index_file)



        Dim index = FilesInpute.Id
        Dim PathFile = FilesInpute.PathFile
        Log_Apllication("Start Compute de " + PathFile)
        Dim myBitmap = New System.Drawing.Bitmap(PathFile)



        Dim Width As Integer = myBitmap.Width
        Dim Height As Integer = myBitmap.Height
        Dim Limite As New Class_Limite(index, FilesInpute.Conf, FilesInpute.Count, Width, Height, Me.PathTempCompute)

        'Vertion PNG
        Dim FramePNG As New Bitmap(Limite.Dif + 1, Limite.H)


        'Signal
        Dim Signal_table As New List(Of Double)
        For m As Integer = 0 To Limite.Dif
            If FilesInpute.Conf.Signal = Signal.Cos Then
                Signal_table.Add((1 - Math.Cos(m * (2 * Math.PI) / (Limite.Dif))) / 2.0)
            ElseIf FilesInpute.Conf.Signal = Signal.Rect Then
                If (Limite.Dif) / 2 < m Then
                    Signal_table.Add(1)
                Else
                    Signal_table.Add(0)
                End If
            Else
                Dim PAs_Signal_Trig As Double = 2.0 / (CDbl(Limite.Dif))
                If Limite.Dif / 2 < m Then
                    PAs_Signal_Trig = -PAs_Signal_Trig
                End If
                If m = 0 Then
                    Signal_table.Add(0.0)
                Else
                    Signal_table.Add(Math.Max(Signal_table.Last + PAs_Signal_Trig, 0.0))
                End If
            End If
        Next


        'Modulation du signale
        If index = 0 Then
            For m As Integer = 0 To CInt(Limite.Dif / 2)
                Signal_table(m) = 1
            Next
        ElseIf index = FilesInpute.Count - 1 Then
            For m As Integer = CInt(Limite.Dif / 2) To Limite.Dif
                Signal_table(m) = 1
            Next
        End If

        For i As Integer = Limite.Start To Limite.Fin
            For k As Integer = 0 To Limite.H - 1
                Dim Pixel As New System.Drawing.Color
                If FilesInpute.Conf.Alignement = Alignement.V Then
                    Pixel = myBitmap.GetPixel(k, i)
                Else
                    Pixel = myBitmap.GetPixel(i, k)
                End If
                FramePNG.SetPixel(i - Limite.Start, k, Color.FromArgb(CInt(Math.Min(Signal_table.Item(i - Limite.Start) * 127, 127)),
                                                               CInt(Math.Min(Pixel.R, 255)),
                                                               CInt(Math.Min(Pixel.G, 255)),
                                                               CInt(Math.Min(Pixel.B, 255))))

            Next
        Next

        FramePNG.Save(Limite.PathFileTemp)
        FramePNG.Dispose()
        myBitmap.Dispose()
        Log_Apllication("End Compute de " + PathFile)
        GC.Collect()

    End Sub

End Class
