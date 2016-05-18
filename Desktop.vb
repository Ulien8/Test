Imports System.Threading
Imports System.Collections
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Collections.Specialized

Public Class Desktop

    Implements IDisposable
    Implements ICloneable
#Region "Imports"
    <DllImport("kernel32.dll")> _
    Private Shared Function GetThreadId(ByVal thread As IntPtr) As Integer
    End Function

    <DllImport("kernel32.dll")> _
    Private Shared Function GetProcessId(ByVal process As IntPtr) As Integer
    End Function

    '
    ' Imported winAPI functions.
    '
    <DllImport("user32.dll")> _
    Private Shared Function CreateDesktop(ByVal lpszDesktop As String, ByVal lpszDevice As IntPtr, ByVal pDevmode As IntPtr, ByVal dwFlags As Integer, ByVal dwDesiredAccess As Integer, ByVal lpsa As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function CloseDesktop(ByVal hDesktop As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function OpenDesktop(ByVal lpszDesktop As String, ByVal dwFlags As Integer, ByVal fInherit As Boolean, ByVal dwDesiredAccess As Integer) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function OpenInputDesktop(ByVal dwFlags As Integer, ByVal fInherit As Boolean, ByVal dwDesiredAccess As Integer) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SwitchDesktop(ByVal hDesktop As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function EnumDesktops(ByVal hwinsta As IntPtr, ByVal lpEnumFunc As EnumDesktopProc, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function GetProcessWindowStation() As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function EnumDesktopWindows(ByVal hDesktop As IntPtr, ByVal lpfn As EnumDesktopWindowsProc, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetThreadDesktop(ByVal hDesktop As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function GetThreadDesktop(ByVal dwThreadId As Integer) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function GetUserObjectInformation(ByVal hObj As IntPtr, ByVal nIndex As Integer, ByVal pvInfo As IntPtr, ByVal nLength As Integer, ByRef lpnLengthNeeded As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll")> _
    Private Shared Function CreateProcess(ByVal lpApplicationName As String, ByVal lpCommandLine As String, ByVal lpProcessAttributes As IntPtr, ByVal lpThreadAttributes As IntPtr, ByVal bInheritHandles As Boolean, ByVal dwCreationFlags As Integer, _
   ByVal lpEnvironment As IntPtr, ByVal lpCurrentDirectory As String, ByRef lpStartupInfo As STARTUPINFO, ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpString As IntPtr, ByVal nMaxCount As Integer) As Integer
    End Function

    Private Delegate Function EnumDesktopProc(ByVal lpszDesktop As String, ByVal lParam As IntPtr) As Boolean
    Private Delegate Function EnumDesktopWindowsProc(ByVal desktopHandle As IntPtr, ByVal lParam As IntPtr) As Boolean

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure PROCESS_INFORMATION
        Public hProcess As IntPtr
        Public hThread As IntPtr
        Public dwProcessId As Integer
        Public dwThreadId As Integer
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure STARTUPINFO
        Public cb As Integer
        Public lpReserved As String
        Public lpDesktop As String
        Public lpTitle As String
        Public dwX As Integer
        Public dwY As Integer
        Public dwXSize As Integer
        Public dwYSize As Integer
        Public dwXCountChars As Integer
        Public dwYCountChars As Integer
        Public dwFillAttribute As Integer
        Public dwFlags As Integer
        Public wShowWindow As Short
        Public cbReserved2 As Short
        Public lpReserved2 As IntPtr
        Public hStdInput As IntPtr
        Public hStdOutput As IntPtr
        Public hStdError As IntPtr
    End Structure
#End Region

#Region "Constants"
    ''' <summary>
    ''' Size of buffer used when retrieving window names.
    ''' </summary>
    Public Const MaxWindowNameLength As Integer = 100

    '
    ' winAPI constants.
    '
    Private Const SW_HIDE As Short = 0
    Private Const SW_NORMAL As Short = 1
    Private Const STARTF_USESTDHANDLES As Integer = &H100
    Private Const STARTF_USESHOWWINDOW As Integer = &H1
    Private Const UOI_NAME As Integer = 2
    Private Const STARTF_USEPOSITION As Integer = &H4
    Private Const NORMAL_PRIORITY_CLASS As Integer = &H20
    Private Const DESKTOP_CREATEWINDOW As Long = &H2L
    Private Const DESKTOP_ENUMERATE As Long = &H40L
    Private Const DESKTOP_WRITEOBJECTS As Long = &H80L
    Private Const DESKTOP_SWITCHDESKTOP As Long = &H100L
    Private Const DESKTOP_CREATEMENU As Long = &H4L
    Private Const DESKTOP_HOOKCONTROL As Long = &H8L
    Private Const DESKTOP_READOBJECTS As Long = &H1L
    Private Const DESKTOP_JOURNALRECORD As Long = &H10L
    Private Const DESKTOP_JOURNALPLAYBACK As Long = &H20L
    Private Const AccessRights As Long = DESKTOP_JOURNALRECORD Or DESKTOP_JOURNALPLAYBACK Or DESKTOP_CREATEWINDOW Or DESKTOP_ENUMERATE Or DESKTOP_WRITEOBJECTS Or DESKTOP_SWITCHDESKTOP Or DESKTOP_CREATEMENU Or DESKTOP_HOOKCONTROL Or DESKTOP_READOBJECTS
#End Region

#Region "Structures"
    ''' <summary>
    ''' ウィンドウのハンドルとタイトルを格納します。
    ''' </summary>
    Public Structure Window

#Region "Private Variables"
        Private m_handle As IntPtr
        Private m_text As String
#End Region

#Region "Public Properties"
        ''' <summary>
        ''' Gets the window handle.
        ''' </summary>
        Public ReadOnly Property Handle() As IntPtr
            Get
                Return m_handle
            End Get
        End Property

        ''' <summary>
        ''' Gets teh window title.
        ''' </summary>
        Public ReadOnly Property Text() As String
            Get
                Return m_text
            End Get
        End Property
#End Region

#Region "Construction"
        ''' <summary>
        ''' Creates a new window object.
        ''' </summary>
        ''' <param name="handle">Window handle.</param>
        ''' <param name="text">Window title.</param>
        Public Sub New(ByVal handle As IntPtr, ByVal text As String)
            m_handle = handle
            m_text = text
        End Sub
#End Region
    End Structure

    ''' <summary>
    ''' ウィンドウオブジェクトのコレクション。
    ''' </summary>
    Public Class WindowCollection
        Inherits CollectionBase
#Region "Public Properties"
        ''' <summary>
        ''' Gets a window from teh collection.
        ''' </summary>
        Default Public ReadOnly Property Item(ByVal index As Integer) As Window
            Get
                Return CType(List(index), Window)
            End Get
        End Property
#End Region

#Region "Methods"
        ''' <summary>
        ''' Adds a window to the collection.
        ''' </summary>
        ''' <param name="wnd">Window to add.</param>
        Public Sub Add(ByVal wnd As Window)
            ' adds a widow to the collection.
            List.Add(wnd)
        End Sub
#End Region
    End Class
#End Region

#Region "Private Variables"
    Private m_desktop As IntPtr
    Private m_desktopName As String
    Private Shared m_sc As StringCollection
    Private m_windows As ArrayList
    Private m_disposed As Boolean
    ' かいぞう
    Private _proc As List(Of Process)
#End Region

#Region "Public Properties"
    ''' <summary>
    ''' デスクトップが開かれているかどうかの判定値を返します。
    ''' </summary>
    Public ReadOnly Property IsOpen() As Boolean
        Get
            Return (m_desktop <> IntPtr.Zero)
        End Get
    End Property

    ''' <summary>
    ''' デスクトップの名前を取得します。（存在しない時はNULL）
    ''' </summary>
    Public ReadOnly Property DesktopName() As String
        Get
            Return m_desktopName
        End Get
    End Property

    ''' <summary>
    ''' デスクトップのハンドルを取得します。（デスクトップが開かれていない時はIntPtr.Zero）
    ''' </summary>
    Public ReadOnly Property DesktopHandle() As IntPtr
        Get
            Return m_desktop
        End Get
    End Property

    ''' <summary>
    ''' デフォルトでメインになっているデスクトップを開きます。
    ''' </summary>
    Public Shared ReadOnly [Default] As Desktop = Desktop.OpenDefaultDesktop()

    ''' <summary>
    ''' ユーザが見ているデスクトップを返します。
    ''' </summary>
    Public Shared ReadOnly Input As Desktop = Desktop.OpenInputDesktop()

    ''' <summary>
    ''' 改造　起動したプロセスを取得します
    ''' </summary>
    Public ReadOnly Property ProcessList() As List(Of Process)
        Get
            Return _proc
        End Get
    End Property

#End Region

#Region "Construction/Destruction"
    ''' <summary>
    ''' 新しいDesktopオブジェクトを作成します。
    ''' </summary>
    Public Sub New()
        ' init variables.
        m_desktop = IntPtr.Zero
        m_desktopName = [String].Empty
        m_windows = New ArrayList()
        m_disposed = False
        ' 改造
        _proc = New List(Of Process)
    End Sub

    ' constructor is private to prevent invalid handles being passed to it.
    Private Sub New(ByVal desktop__1 As IntPtr)
        ' init variables.
        m_desktop = desktop__1
        m_desktopName = Desktop.GetDesktopName(desktop__1)
        m_windows = New ArrayList()
        m_disposed = False
        ' 改造
        _proc = New List(Of Process)
    End Sub

    Protected Overrides Sub Finalize()
        Try
            ' clean up, close the desktop.
            Close()
        Finally
            MyBase.Finalize()
        End Try
    End Sub
#End Region

#Region "Methods"
    ''' <summary>
    ''' 新しいデスクトップを作成します。ハンドルが開かれているときはそれを閉じます。
    ''' </summary>
    ''' <param name="name">新しいデスクトップの名前。一意なもので大文字と小文字は区別されます。</param>
    ''' <returns>デスクトップの作成に成功した時はTrueを返します。それ以外の場合はFalseを返します。</returns>
    Public Function Create(ByVal name As String) As Boolean
        ' make sure object isnt disposed.
        CheckDisposed()

        ' close the open desktop.
        If m_desktop <> IntPtr.Zero Then
            ' attempt to close the desktop.
            If Not Close() Then
                Return False
            End If
        End If

        ' make sure desktop doesnt already exist.
        If desktop.Exists(name) Then
            ' it exists, so open it.
            Return Open(name)
        End If

        ' attempt to create desktop.
        m_desktop = CreateDesktop(name, IntPtr.Zero, IntPtr.Zero, 0, AccessRights, IntPtr.Zero)

        m_desktopName = name

        ' something went wrong.
        If m_desktop = IntPtr.Zero Then
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    ''' デスクトップのハンドルを閉じます。
    ''' </summary>
    ''' <returns>開いているハンドルを閉じるのに成功した時にTrueを返します。</returns>
    Public Function Close() As Boolean
        ' make sure object isnt disposed.
        CheckDisposed()

        ' check there is a desktop open.
        If m_desktop <> IntPtr.Zero Then
            ' close the desktop.
            Dim result As Boolean = CloseDesktop(m_desktop)

            If result Then
                m_desktop = IntPtr.Zero

                m_desktopName = [String].Empty
            End If

            Return result
        End If

        ' no desktop was open, so desktop is closed.
        Return True
    End Function

    ''' <summary>
    ''' デスクトップを開きます。
    ''' </summary>
    ''' <param name="name">開こうとするデスクトップの名前。</param>
    ''' <returns>デスクトップを開くのに成功した時にTrueを返します。</returns>
    Public Function Open(ByVal name As String) As Boolean
        ' make sure object isnt disposed.
        CheckDisposed()

        ' close the open desktop.
        If m_desktop <> IntPtr.Zero Then
            ' attempt to close the desktop.
            If Not Close() Then
                Return False
            End If
        End If

        ' open the desktop.
        m_desktop = OpenDesktop(name, 0, True, AccessRights)

        ' something went wrong.
        If m_desktop = IntPtr.Zero Then
            Return False
        End If

        m_desktopName = name

        Return True
    End Function

    ''' <summary>
    ''' 現在の入力デスクトップを開きます。
    ''' </summary>
    ''' <returns>デスクトップを開くのに成功した時にTrueを返します。</returns>
    Public Function OpenInput() As Boolean
        ' make sure object isnt disposed.
        CheckDisposed()

        ' close the open desktop.
        If m_desktop <> IntPtr.Zero Then
            ' attempt to close the desktop.
            If Not Close() Then
                Return False
            End If
        End If

        ' open the desktop.
        m_desktop = OpenInputDesktop(0, True, AccessRights)

        ' something went wrong.
        If m_desktop = IntPtr.Zero Then
            Return False
        End If

        ' get the desktop name.
        m_desktopName = desktop.GetDesktopName(m_desktop)

        Return True
    End Function

    ''' <summary>
    ''' 入力を現在の開いているデスクトップに移します。
    ''' </summary>
    ''' <returns>デスクトップの切り替えが成功した時にTrueを返します。</returns>
    Public Function Show() As Boolean
        ' make sure object isnt disposed.
        CheckDisposed()

        ' make sure there is a desktop to open.
        If m_desktop = IntPtr.Zero Then
            Return False
        End If

        ' attempt to switch desktops.
        Dim result As Boolean = SwitchDesktop(m_desktop)

        Return result
    End Function

    ''' <summary>
    ''' デスクトップ上のウィンドウを列挙します。
    ''' </summary>
    ''' <returns>成功した時はウィンドウコレクションを、それ以外はNULLを返します。</returns>
    Public Function GetWindows() As WindowCollection
        ' make sure object isnt disposed.
        CheckDisposed()

        ' make sure a desktop is open.
        If Not IsOpen Then
            Return Nothing
        End If

        ' init the arraylist.
        m_windows.Clear()
        Dim windows As New WindowCollection()

        ' get windows.
        Dim result As Boolean = EnumDesktopWindows(m_desktop, New EnumDesktopWindowsProc(AddressOf DesktopWindowsProc), IntPtr.Zero)

        ' check for error.
        If Not result Then
            Return Nothing
        End If

        ' get window names.
        windows = New WindowCollection()

        Dim ptr As IntPtr = Marshal.AllocHGlobal(MaxWindowNameLength)

        For Each wnd As IntPtr In m_windows
            GetWindowText(wnd, ptr, MaxWindowNameLength)
            windows.Add(New Window(wnd, Marshal.PtrToStringAnsi(ptr)))
        Next

        Marshal.FreeHGlobal(ptr)

        Return windows
    End Function

    Private Function DesktopWindowsProc(ByVal wndHandle As IntPtr, ByVal lParam As IntPtr) As Boolean
        ' add window handle to colleciton.
        m_windows.Add(wndHandle)

        Return True
    End Function

    ''' <summary>
    ''' デスクトップで新しいプロセスを作成して実行します。
    ''' </summary>
    ''' <param name="path">アプリケーションのパス。</param>
    ''' <returns>新しく作成したプロセスのオブジェクトを返します。</returns>
    Public Function CreateProcess(ByVal path As String) As Process
        ' make sure object isnt disposed.
        CheckDisposed()

        ' make sure a desktop is open.
        If Not IsOpen Then
            Return Nothing
        End If

        ' set startup parameters.
        Dim si As New STARTUPINFO()
        si.cb = Marshal.SizeOf(si)
        si.lpDesktop = m_desktopName

        Dim pi As New PROCESS_INFORMATION()

        ' start the process.
        Dim result As Boolean = CreateProcess(Nothing, path, IntPtr.Zero, IntPtr.Zero, True, NORMAL_PRIORITY_CLASS, _
         IntPtr.Zero, Nothing, si, pi)

        ' error?
        If Not result Then
            Return Nothing
        End If

        ' Get the process.
        ' 改造　プロセスを保持しておく
        Dim p As Process = Process.GetProcessById(pi.dwProcessId)
        _proc.Add(p)
        Return p
    End Function


    Public Function xCreateProcess(ByVal path As String) As Process
        ' make sure object isnt disposed.
        CheckDisposed()

        ' make sure a desktop is open.
        If Not IsOpen Then
            Return Nothing
        End If

        ' set startup parameters.
        Dim si As New STARTUPINFO()
        si.cb = Marshal.SizeOf(si)
        si.lpDesktop = m_desktopName

        Dim pi As New PROCESS_INFORMATION()

        ' start the process.
        Dim result As Boolean = CreateProcess(Nothing, path, IntPtr.Zero, IntPtr.Zero, True, NORMAL_PRIORITY_CLASS, _
         IntPtr.Zero, Nothing, si, pi)

        ' error?
        If Not result Then
            Return Nothing
        End If

        ' Get the process.
        Return Process.GetProcessById(pi.dwProcessId)
    End Function

    ''' <summary>
    ''' 使用するデスクトップの準備をします。新しいデスクトップを作成したCreateDesktopの後で呼び出します。
    ''' </summary>
    Public Sub Prepare()
        ' make sure object isnt disposed.
        CheckDisposed()

        ' make sure a desktop is open.
        If IsOpen Then
            ' load explorer.
            CreateProcess("explorer.exe")

        End If

    End Sub

#End Region

#Region "Static Methods"
    ''' <summary>
    ''' 全てのデスクトップを列挙します。
    ''' </summary>
    ''' <returns>デスクトップの名前の列挙に成功した時にTrueを返します。</returns>
    Public Shared Function GetDesktops() As String()
        ' attempt to enum desktops.
        Dim windowStation As IntPtr = GetProcessWindowStation()

        ' check we got a valid handle.
        If windowStation = IntPtr.Zero Then
            Return New String(-1) {}
        End If

        Dim desktops As String()

        ' lock the object. thread safety and all.
        SyncLock InlineAssignHelper(m_sc, New StringCollection())
            Dim result As Boolean = EnumDesktops(windowStation, New EnumDesktopProc(AddressOf DesktopProc), IntPtr.Zero)

            ' something went wrong.
            If Not result Then
                Return New String(-1) {}
            End If

            '	// turn the collection into an array.
            desktops = New String(m_sc.Count - 1) {}
            For i As Integer = 0 To desktops.Length - 1
                desktops(i) = m_sc(i)
            Next
        End SyncLock

        Return desktops
    End Function

    Private Shared Function DesktopProc(ByVal lpszDesktop As String, ByVal lParam As IntPtr) As Boolean
        ' add the desktop to the collection.
        m_sc.Add(lpszDesktop)

        Return True
    End Function

    ''' <summary>
    ''' 明示されたデスクトップに切り替えます。
    ''' </summary>
    ''' <param name="name">切り替える入力デスクトップの名前。</param>
    ''' <returns>切り替えが成功した時にTrueを返します。</returns>
    Public Shared Function Show(ByVal name As String) As Boolean
        ' attmempt to open desktop.
        Dim result As Boolean = False

        Dim d As New Desktop()
        result = d.Open(name)

        ' something went wrong.
        If Not result Then
            Return False
        End If

        ' attempt to switch desktops.
        result = d.Show()

        Return result
    End Function

    ''' <summary>
    ''' スレッドに呼び出されているデスクトップを取得します。
    ''' </summary>
    ''' <returns>スレッドの（valling?）Desktopオブジェクトを返します。</returns>
    Public Shared Function GetCurrent() As Desktop
        ' get the desktop.
        Return New Desktop(GetThreadDesktop(AppDomain.GetCurrentThreadId()))
    End Function

    ''' <summary>
    ''' スレッドに呼び出されるデスクトップを設定します。
    ''' NOTE: Function will fail if thread has hooks or windows in the current desktop.
    ''' </summary>
    ''' <param name="desktop">スレッドに関連付けるデスクトップ。</param>
    ''' <returns>スレッドへの設定に成功した時にTrueを返します。</returns>
    Public Shared Function SetCurrent(ByVal desktop As Desktop) As Boolean
        ' set threads desktop.
        If Not desktop.IsOpen Then
            Return False
        End If

        Return SetThreadDesktop(desktop.DesktopHandle)
    End Function

    ''' <summary>
    ''' デスクトップを開きます。
    ''' </summary>
    ''' <param name="name">開こうとするデスクトップの名前。</param>
    ''' <returns>成功ならそのデスクトップオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function OpenDesktop(ByVal name As String) As Desktop
        ' open the desktop.
        Dim desktop As New Desktop()
        Dim result As Boolean = desktop.Open(name)

        ' somethng went wrong.
        If Not result Then
            Return Nothing
        End If

        Return desktop
    End Function

    ''' <summary>
    ''' 現在の入力デスクトップを開きます。
    ''' </summary>
    ''' <returns>成功した時はDesktopオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function OpenInputDesktop() As Desktop
        ' open the desktop.
        Dim desktop As New Desktop()
        Dim result As Boolean = desktop.OpenInput()

        ' somethng went wrong.
        If Not result Then
            Return Nothing
        End If

        Return desktop
    End Function

    ''' <summary>
    ''' デフォルトのデスクトップを開きます。
    ''' </summary>
    ''' <returns>成功した時はDesktopオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function OpenDefaultDesktop() As Desktop
        ' opens the default desktop.
        Return Desktop.OpenDesktop("Default")
    End Function

    ''' <summary>
    ''' 新しいデスクトップを作成します。
    ''' </summary>
    ''' <param name="name">新しいデスクトップの名前。大文字と小文字は区別されます。</param>
    ''' <returns>成功の場合はDesktopオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function CreateDesktop(ByVal name As String) As Desktop
        ' open the desktop.
        Dim desktop As New Desktop()
        Dim result As Boolean = desktop.Create(name)

        ' somethng went wrong.
        If Not result Then
            Return Nothing
        End If

        Return desktop
    End Function

    ''' <summary>
    ''' 指定したデスクトップの名前を取得します。
    ''' </summary>
    ''' <param name="desktop">Desktopオブジェクト。whos name is to be found.</param>
    ''' <returns>成功の場合はデスクトップの名前を、それ以外はNULLを返します。</returns>
    Public Shared Function GetDesktopName(ByVal desktop As Desktop) As String
        ' get name.
        If desktop.IsOpen Then
            Return Nothing
        End If

        Return GetDesktopName(desktop.DesktopHandle)
    End Function

    ''' <summary>
    ''' ハンドルからデスクトップの名前を取得します。
    ''' </summary>
    ''' <param name="desktopHandle"></param>
    ''' <returns>成功した時はDesktopオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function GetDesktopName(ByVal desktopHandle As IntPtr) As String
        ' check its not a null pointer.
        ' null pointers wont work.
        If desktopHandle = IntPtr.Zero Then
            Return Nothing
        End If

        ' get the length of the name.
        Dim needed As Integer = 0
        Dim name As String = [String].Empty
        GetUserObjectInformation(desktopHandle, UOI_NAME, IntPtr.Zero, 0, needed)

        ' get the name.
        Dim ptr As IntPtr = Marshal.AllocHGlobal(needed)
        Dim result As Boolean = GetUserObjectInformation(desktopHandle, UOI_NAME, ptr, needed, needed)
        name = Marshal.PtrToStringAnsi(ptr)
        Marshal.FreeHGlobal(ptr)

        ' something went wrong.
        If Not result Then
            Return Nothing
        End If

        Return name
    End Function

    ''' <summary>
    ''' 明示されたデスクトップが存在するか確認します。（大文字と小文字を区別する検索を使用します。）
    ''' </summary>
    ''' <param name="name">デスクトップの名前。</param>
    ''' <returns>指定されたデスクトップが存在する時はTrue、それ以外はFalseを返します。</returns>
    Public Shared Function Exists(ByVal name As String) As Boolean
        Return Desktop.Exists(name, False)
    End Function

    ''' <summary>
    ''' 明示されたデスクトップが存在するか確認します。
    ''' </summary>
    ''' <param name="name">デスクトップの名前。</param>
    ''' <param name="caseInsensitive">大文字と小文字を区別しない検索を使用する時はTrueにします。</param>
    ''' <returns>指定されたデスクトップが存在する時はTrue、それ以外はFalseを返します。</returns>
    Public Shared Function Exists(ByVal name As String, ByVal caseInsensitive As Boolean) As Boolean
        ' enumerate desktops.
        Dim desktops As String() = Desktop.GetDesktops()

        ' return true if desktop exists.
        For Each desktop__1 As String In desktops
            If caseInsensitive Then
                ' case insensitive, compare all in lower case.
                If desktop__1.ToLower() = name.ToLower() Then
                    Return True
                End If
            Else
                If desktop__1 = name Then
                    Return True
                End If
            End If
        Next

        Return False
    End Function

    ''' <summary>
    ''' 明示されたデスクトップに新しいプロセスを作成します。
    ''' </summary>
    ''' <param name="path">アプリケーションのパス。</param>
    ''' <param name="desktop__1">デスクトップの名前。</param>
    ''' <returns>新しく作成された時はプロセスのオブジェクトを、それ以外はNULLを返します。</returns>
    Public Shared Function CreateProcess(ByVal path As String, ByVal desktop__1 As String) As Process
        If Not Desktop.Exists(desktop__1) Then
            Return Nothing
        End If

        ' create the process.
        Dim d As Desktop = Desktop.OpenDesktop(desktop__1)
        Return d.CreateProcess(path)
    End Function

    ''' <summary>
    ''' 入力デスクトップで実行中の全てのプロセスを配列として取得します。
    ''' </summary>
    ''' <returns>プロセスが格納された配列を返します。</returns>
    Public Shared Function GetInputProcesses() As Process()
        ' get all processes.
        Dim processes As Process() = Process.GetProcesses()

        Dim m_procs As New ArrayList()

        ' get the current desktop name.
        Dim currentDesktop As String = GetDesktopName(Desktop.Input.DesktopHandle)

        ' cycle through the processes.
        For Each process__1 As Process In processes
            ' check the threads of the process - are they in this one?
            For Each pt As ProcessThread In process__1.Threads
                ' check for a desktop name match.
                If GetDesktopName(GetThreadDesktop(pt.Id)) = currentDesktop Then
                    ' found a match, add to list, and bail.
                    m_procs.Add(process__1)
                    Exit For
                End If
            Next
        Next

        ' put ArrayList into array.
        Dim procs As Process() = New Process(m_procs.Count - 1) {}

        For i As Integer = 0 To procs.Length - 1
            procs(i) = DirectCast(m_procs(i), Process)
        Next

        Return procs
    End Function
#End Region

#Region "IDisposable"
    ''' <summary>
    ''' Dispose Object.
    ''' </summary>
    Public Interface IDisposable

    End Interface
    Public Interface ICloneable

    End Interface
    Public Sub Dispose()
        ' dispose
        Dispose(True)

        ' suppress finalisation
        GC.SuppressFinalize(Me)
    End Sub
    ''' <summary>
    ''' オブジェクトを配置します。
    ''' </summary>
    ''' <param name="disposing">True to dispose managed resources.</param>
    Public Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not m_disposed Then
            ' dispose of managed resources,
            ' close handles
            Close()
        End If

        m_disposed = True
    End Sub

    Private Sub CheckDisposed()
        ' check if disposed
        If m_disposed Then
            ' object disposed, throw exception
            Throw New ObjectDisposedException("")
        End If
    End Sub
#End Region

#Region "ICloneable"
    ''' <summary>
    ''' 開かれているデスクトップと同じDesktopオブジェクトを作成します。
    ''' </summary>
    ''' <returns>Desktopオブジェクトのクローンを返します。</returns>
    Public Function Clone() As Object
        ' make sure object isnt disposed.
        CheckDisposed()

        Dim desktop As New Desktop()

        ' if a desktop is open, make the clone open it.
        If IsOpen Then
            desktop.Open(m_desktopName)
        End If

        Return desktop
    End Function
#End Region

#Region "Overrides"
    ''' <summary>
    ''' デスクトップの名前を取得します。
    ''' </summary>
    ''' <returns>デスクトップが開かれていない時は空白を、それ以外はデスクトップの名前を返します。</returns>
    Public Overrides Function ToString() As String
        ' return the desktop name.
        Return m_desktopName
    End Function
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
#End Region


End Class
