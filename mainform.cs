using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortEnhancedGUI
{
    public class EnhancedSerialForm : Form
    {
        // Serial port objects for COM1 and COM2
        private SerialPort port1;
        private SerialPort port2;

        // Controls for Port 1
        private GroupBox groupBoxPort1;
        private Label lblPort1Status;
        private TextBox txtPort1Send;
        private Button btnPort1Open;
        private Button btnPort1Close;
        private Button btnPort1Send;
        private TextBox txtPort1Receive;

        // Controls for Port 2
        private GroupBox groupBoxPort2;
        private Label lblPort2Status;
        private TextBox txtPort2Send;
        private Button btnPort2Open;
        private Button btnPort2Close;
        private Button btnPort2Send;
        private TextBox txtPort2Receive;

        // TabControl for Global Log only
        private TabControl tabControlResults;
        private TabPage tabPageLog;
        private TextBox txtLog;

        // Test control buttons and summary
        private Button btnTest;
        private Button btnPause;
        private Button btnStop;
        private TextBox txtTestSummary;

        // Real-time test timer display
        private Label lblTestTime;

        // UI timer (using Windows Forms Timer)
        private System.Windows.Forms.Timer uiTimer;

        // Cancellation token and pause flag for test procedure
        private CancellationTokenSource testCts;
        private volatile bool isPaused;

        // Stopwatch for test timing
        private Stopwatch testStopwatch;

        public EnhancedSerialForm()
        {
            // Set up form properties
            this.Text = "Enhanced Serial Communication GUI";
            this.Size = new Size(800, 680);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponents();
            InitializeSerialPorts();
        }

        private void InitializeComponents()
        {
            // ---------------------------
            // Group Box for Port 1 (COM1)
            // ---------------------------
            groupBoxPort1 = new GroupBox();
            groupBoxPort1.Text = "Port 1 (COM1)";
            groupBoxPort1.Size = new Size(370, 200);
            groupBoxPort1.Location = new Point(10, 10);

            lblPort1Status = new Label();
            lblPort1Status.Text = "Status: Closed";
            lblPort1Status.Location = new Point(10, 25);
            lblPort1Status.AutoSize = true;

            btnPort1Open = new Button();
            btnPort1Open.Text = "Open";
            btnPort1Open.Location = new Point(250, 20);
            btnPort1Open.Size = new Size(100, 25);
            btnPort1Open.Click += BtnPort1Open_Click;

            btnPort1Close = new Button();
            btnPort1Close.Text = "Close";
            btnPort1Close.Location = new Point(250, 55);
            btnPort1Close.Size = new Size(100, 25);
            btnPort1Close.Click += BtnPort1Close_Click;

            Label lblPort1Send = new Label();
            lblPort1Send.Text = "Send:";
            lblPort1Send.Location = new Point(10, 60);
            lblPort1Send.AutoSize = true;

            txtPort1Send = new TextBox();
            txtPort1Send.Location = new Point(70, 60);
            txtPort1Send.Size = new Size(160, 25);

            btnPort1Send = new Button();
            btnPort1Send.Text = "Send";
            btnPort1Send.Location = new Point(250, 90);
            btnPort1Send.Size = new Size(100, 25);
            btnPort1Send.Click += BtnPort1Send_Click;

            Label lblPort1Receive = new Label();
            lblPort1Receive.Text = "Receive:";
            lblPort1Receive.Location = new Point(10, 130);
            lblPort1Receive.AutoSize = true;

            txtPort1Receive = new TextBox();
            txtPort1Receive.Location = new Point(70, 125);
            txtPort1Receive.Size = new Size(280, 25);
            txtPort1Receive.ReadOnly = true;

            groupBoxPort1.Controls.Add(lblPort1Status);
            groupBoxPort1.Controls.Add(btnPort1Open);
            groupBoxPort1.Controls.Add(btnPort1Close);
            groupBoxPort1.Controls.Add(lblPort1Send);
            groupBoxPort1.Controls.Add(txtPort1Send);
            groupBoxPort1.Controls.Add(btnPort1Send);
            groupBoxPort1.Controls.Add(lblPort1Receive);
            groupBoxPort1.Controls.Add(txtPort1Receive);

            // ---------------------------
            // Group Box for Port 2 (COM2)
            // ---------------------------
            groupBoxPort2 = new GroupBox();
            groupBoxPort2.Text = "Port 2 (COM2)";
            groupBoxPort2.Size = new Size(370, 200);
            groupBoxPort2.Location = new Point(400, 10);

            lblPort2Status = new Label();
            lblPort2Status.Text = "Status: Closed";
            lblPort2Status.Location = new Point(10, 25);
            lblPort2Status.AutoSize = true;

            btnPort2Open = new Button();
            btnPort2Open.Text = "Open";
            btnPort2Open.Location = new Point(250, 20);
            btnPort2Open.Size = new Size(100, 25);
            btnPort2Open.Click += BtnPort2Open_Click;

            btnPort2Close = new Button();
            btnPort2Close.Text = "Close";
            btnPort2Close.Location = new Point(250, 55);
            btnPort2Close.Size = new Size(100, 25);
            btnPort2Close.Click += BtnPort2Close_Click;

            Label lblPort2Send = new Label();
            lblPort2Send.Text = "Send:";
            lblPort2Send.Location = new Point(10, 60);
            lblPort2Send.AutoSize = true;

            txtPort2Send = new TextBox();
            txtPort2Send.Location = new Point(70, 60);
            txtPort2Send.Size = new Size(160, 25);

            btnPort2Send = new Button();
            btnPort2Send.Text = "Send";
            btnPort2Send.Location = new Point(250, 90);
            btnPort2Send.Size = new Size(100, 25);
            btnPort2Send.Click += BtnPort2Send_Click;

            Label lblPort2Receive = new Label();
            lblPort2Receive.Text = "Receive:";
            lblPort2Receive.Location = new Point(10, 130);
            lblPort2Receive.AutoSize = true;

            txtPort2Receive = new TextBox();
            txtPort2Receive.Location = new Point(70, 125);
            txtPort2Receive.Size = new Size(280, 25);
            txtPort2Receive.ReadOnly = true;

            groupBoxPort2.Controls.Add(lblPort2Status);
            groupBoxPort2.Controls.Add(btnPort2Open);
            groupBoxPort2.Controls.Add(btnPort2Close);
            groupBoxPort2.Controls.Add(lblPort2Send);
            groupBoxPort2.Controls.Add(txtPort2Send);
            groupBoxPort2.Controls.Add(btnPort2Send);
            groupBoxPort2.Controls.Add(lblPort2Receive);
            groupBoxPort2.Controls.Add(txtPort2Receive);

            // ---------------------------
            // TabControl for Global Log only
            // ---------------------------
            tabControlResults = new TabControl();
            tabControlResults.Location = new Point(10, 220);
            tabControlResults.Size = new Size(760, 200);

            tabPageLog = new TabPage("Global Log");
            txtLog = new TextBox();
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.Dock = DockStyle.Fill;
            tabPageLog.Controls.Add(txtLog);
            tabControlResults.TabPages.Add(tabPageLog);

            // ---------------------------
            // Test Buttons and Test Summary TextBox
            // ---------------------------
            btnTest = new Button();
            btnTest.Text = "Run Test";
            btnTest.Location = new Point(10, 430);
            btnTest.Size = new Size(100, 30);
            btnTest.Click += BtnTest_Click;

            btnPause = new Button();
            btnPause.Text = "Pause";
            btnPause.Location = new Point(120, 430);
            btnPause.Size = new Size(100, 30);
            btnPause.Click += BtnPause_Click;

            btnStop = new Button();
            btnStop.Text = "Stop";
            btnStop.Location = new Point(230, 430);
            btnStop.Size = new Size(100, 30);
            btnStop.Click += BtnStop_Click;

            txtTestSummary = new TextBox();
            txtTestSummary.Multiline = true;
            txtTestSummary.ScrollBars = ScrollBars.Vertical;
            txtTestSummary.ReadOnly = true;
            txtTestSummary.Location = new Point(10, 470);
            txtTestSummary.Size = new Size(760, 80);

            // ---------------------------
            // Test Time Label (real-time display)
            // ---------------------------
            lblTestTime = new Label();
            lblTestTime.Text = "Test Time: 00:00:00";
            lblTestTime.Location = new Point(10, 560);
            lblTestTime.AutoSize = true;
            lblTestTime.Font = new Font("Arial", 10, FontStyle.Bold);

            // ---------------------------
            // UI Timer to update test time every 100ms
            // ---------------------------
            uiTimer = new System.Windows.Forms.Timer();
            uiTimer.Interval = 100; // 100ms
            uiTimer.Tick += (s, e) =>
            {
                if (testStopwatch != null && testStopwatch.IsRunning)
                {
                    lblTestTime.Text = "Test Time: " + testStopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                }
            };

            // Add all controls to the form
            this.Controls.Add(groupBoxPort1);
            this.Controls.Add(groupBoxPort2);
            this.Controls.Add(tabControlResults);
            this.Controls.Add(btnTest);
            this.Controls.Add(btnPause);
            this.Controls.Add(btnStop);
            this.Controls.Add(txtTestSummary);
            this.Controls.Add(lblTestTime);
        }

        private void InitializeSerialPorts()
        {
            // Initialize COM1
            port1 = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            port1.DataReceived += Port1_DataReceived;

            // Initialize COM2 with the global event handler
            port2 = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);
            port2.DataReceived += Port2_DataReceived;
        }

        // Global event handler for Port1 DataReceived
        private void Port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = port1.ReadLine();
                this.Invoke(new Action(() =>
                {
                    txtPort1Receive.Text = data;
                    AppendLog($"Port1 received: {data}");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => AppendLog($"Error reading Port1: {ex.Message}")));
            }
        }

        // Global event handler for Port2 DataReceived (non-test messages)
        private void Port2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = port2.ReadLine();
                this.Invoke(new Action(() =>
                {
                    txtPort2Receive.Text = data;
                    AppendLog($"Port2 received: {data}");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() => AppendLog($"Error reading Port2: {ex.Message}")));
            }
        }

        // Append message to the global log
        private void AppendLog(string message)
        {
            txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");
        }

        // ---------------------------
        // Button Click Handlers for Port1
        // ---------------------------
        private void BtnPort1Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port1.IsOpen)
                {
                    port1.Open();
                    lblPort1Status.Text = "Status: Open";
                    AppendLog("Port1 opened.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error opening Port1: {ex.Message}");
            }
        }

        private void BtnPort1Close_Click(object sender, EventArgs e)
        {
            try
            {
                if (port1.IsOpen)
                {
                    port1.Close();
                    lblPort1Status.Text = "Status: Closed";
                    AppendLog("Port1 closed.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error closing Port1: {ex.Message}");
            }
        }

        private void BtnPort1Send_Click(object sender, EventArgs e)
        {
            if (port1.IsOpen)
            {
                try
                {
                    string data = txtPort1Send.Text;
                    port1.WriteLine(data);
                    AppendLog($"Port1 sent: {data}");
                }
                catch (Exception ex)
                {
                    AppendLog($"Error sending from Port1: {ex.Message}");
                }
            }
            else
            {
                AppendLog("Port1 is not open.");
            }
        }

        // ---------------------------
        // Button Click Handlers for Port2
        // ---------------------------
        private void BtnPort2Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (!port2.IsOpen)
                {
                    port2.Open();
                    lblPort2Status.Text = "Status: Open";
                    AppendLog("Port2 opened.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error opening Port2: {ex.Message}");
            }
        }

        private void BtnPort2Close_Click(object sender, EventArgs e)
        {
            try
            {
                if (port2.IsOpen)
                {
                    port2.Close();
                    lblPort2Status.Text = "Status: Closed";
                    AppendLog("Port2 closed.");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error closing Port2: {ex.Message}");
            }
        }

        private void BtnPort2Send_Click(object sender, EventArgs e)
        {
            if (port2.IsOpen)
            {
                try
                {
                    string data = txtPort2Send.Text;
                    port2.WriteLine(data);
                    AppendLog($"Port2 sent: {data}");
                }
                catch (Exception ex)
                {
                    AppendLog($"Error sending from Port2: {ex.Message}");
                }
            }
            else
            {
                AppendLog("Port2 is not open.");
            }
        }

        // ---------------------------
        // Test Procedure Button Click Handler
        // ---------------------------
        private async void BtnTest_Click(object sender, EventArgs e)
        {
            // If a test is already running, ignore a new Run request.
            if (testCts != null && !testCts.IsCancellationRequested)
                return;

            // Reset pause flag and cancellation token; reset stopwatch.
            isPaused = false;
            btnPause.Text = "Pause";
            testCts = new CancellationTokenSource();
            testStopwatch = new Stopwatch();
            testStopwatch.Start();
            uiTimer.Start();

            AppendLog("Starting test procedure...");
            txtTestSummary.Clear(); // Clear previous summary

            // Auto-open both ports if not already open
            if (!port1.IsOpen)
            {
                try { port1.Open(); lblPort1Status.Text = "Status: Open"; AppendLog("Port1 auto-opened for test."); }
                catch (Exception ex) { AppendLog($"Error auto-opening Port1: {ex.Message}"); return; }
            }
            if (!port2.IsOpen)
            {
                try { port2.Open(); lblPort2Status.Text = "Status: Open"; AppendLog("Port2 auto-opened for test."); }
                catch (Exception ex) { AppendLog($"Error auto-opening Port2: {ex.Message}"); return; }
            }

            // Initialize counters for each phase
            int phase1Pass = 0, phase1Total = 25;
            int phase2Pass = 0, phase2Total = 25;

            try
            {
                // ------------- Phase 1: Test from Port1 to Port2 (Steps 1 to 25) -------------
                for (int i = 1; i <= phase1Total; i++)
                {
                    testCts.Token.ThrowIfCancellationRequested();
                    // Wait while paused
                    while (isPaused)
                    {
                        await Task.Delay(100, testCts.Token);
                        testCts.Token.ThrowIfCancellationRequested();
                    }

                    string testMessage = $"TEST1234_STEP_{i}";
                    this.Invoke(new Action(() => txtPort2Receive.Text = string.Empty));

                    // Temporarily remove global Port2 handler
                    port2.DataReceived -= Port2_DataReceived;

                    // Await response from Port2
                    var tcs = new TaskCompletionSource<string>();
                    SerialDataReceivedEventHandler testHandler = null;
                    testHandler = (s, evt) =>
                    {
                        try
                        {
                            string received = port2.ReadLine();
                            this.Invoke(new Action(() =>
                            {
                                txtPort2Receive.Text = received;
                            }));
                            tcs.TrySetResult(received);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                        port2.DataReceived -= testHandler;
                    };
                    port2.DataReceived += testHandler;

                    // Send test message from Port1
                    port1.WriteLine(testMessage);
                    AppendLog($"Phase 1 - Step {i}: Sent from Port1: {testMessage}");

                    string receivedTestMessage = "";
                    bool stepPassed = false;
                    var delayTask = Task.Delay(1000, testCts.Token);
                    var completedTask = await Task.WhenAny(tcs.Task, delayTask);
                    if (completedTask == tcs.Task)
                    {
                        receivedTestMessage = tcs.Task.Result;
                        if (receivedTestMessage.Trim() == testMessage.Trim())
                        {
                            stepPassed = true;
                            phase1Pass++;
                        }
                    }
                    else
                    {
                        AppendLog($"Phase 1 - Step {i}: Test timed out waiting for response.");
                    }
                    AppendLog($"Phase 1 - Step {i} result: {(stepPassed ? "Passed" : "Failed")}");
                    // Reattach the global Port2 handler
                    port2.DataReceived += Port2_DataReceived;

                    await Task.Delay(100, testCts.Token);
                }

                // ------------- Phase 2: Test from Port2 to Port1 (Steps 26 to 50) -------------
                for (int i = phase1Total + 1; i <= phase1Total + phase2Total; i++)
                {
                    testCts.Token.ThrowIfCancellationRequested();
                    while (isPaused)
                    {
                        await Task.Delay(100, testCts.Token);
                        testCts.Token.ThrowIfCancellationRequested();
                    }

                    string testMessage = $"TEST1234_STEP_{i}";
                    this.Invoke(new Action(() => txtPort1Receive.Text = string.Empty));

                    // Temporarily remove global Port1 handler
                    port1.DataReceived -= Port1_DataReceived;

                    // Await response from Port1
                    var tcs = new TaskCompletionSource<string>();
                    SerialDataReceivedEventHandler testHandler = null;
                    testHandler = (s, evt) =>
                    {
                        try
                        {
                            string received = port1.ReadLine();
                            this.Invoke(new Action(() =>
                            {
                                txtPort1Receive.Text = received;
                            }));
                            tcs.TrySetResult(received);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                        port1.DataReceived -= testHandler;
                    };
                    port1.DataReceived += testHandler;

                    // Send test message from Port2
                    port2.WriteLine(testMessage);
                    AppendLog($"Phase 2 - Step {i}: Sent from Port2: {testMessage}");

                    string receivedTestMessage = "";
                    bool stepPassed = false;
                    var delayTask2 = Task.Delay(1000, testCts.Token);
                    var completedTask2 = await Task.WhenAny(tcs.Task, delayTask2);
                    if (completedTask2 == tcs.Task)
                    {
                        receivedTestMessage = tcs.Task.Result;
                        if (receivedTestMessage.Trim() == testMessage.Trim())
                        {
                            stepPassed = true;
                            phase2Pass++;
                        }
                    }
                    else
                    {
                        AppendLog($"Phase 2 - Step {i}: Test timed out waiting for response.");
                    }
                    AppendLog($"Phase 2 - Step {i} result: {(stepPassed ? "Passed" : "Failed")}");
                    // Reattach the global Port1 handler
                    port1.DataReceived += Port1_DataReceived;

                    await Task.Delay(100, testCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                AppendLog("Test procedure was stopped.");
            }
            finally
            {
                testStopwatch.Stop();
                uiTimer.Stop();
            }

            // Build overall report summary including total test time
            double phase1SuccessRate = phase1Total > 0 ? (phase1Pass * 100.0 / phase1Total) : 0;
            double phase2SuccessRate = phase2Total > 0 ? (phase2Pass * 100.0 / phase2Total) : 0;
            int overallPass = phase1Pass + phase2Pass;
            int overallTotal = phase1Total + phase2Total;
            double overallSuccessRate = overallTotal > 0 ? (overallPass * 100.0 / overallTotal) : 0;
            string totalTestTime = testStopwatch.Elapsed.ToString(@"hh\:mm\:ss");

            string summary = "";
            summary += $"Phase 1 Test: {phase1Pass}/{phase1Total} ({phase1SuccessRate:F2}% success){Environment.NewLine}";
            summary += $"Phase 2 Test: {phase2Pass}/{phase2Total} ({phase2SuccessRate:F2}% success){Environment.NewLine}";
            summary += $"Overall: {overallPass}/{overallTotal} ({overallSuccessRate:F2}% success){Environment.NewLine}";
            summary += $"Total Test Time: {totalTestTime}";

            AppendLog("Test procedure completed.");
            txtTestSummary.Text = summary;
            lblTestTime.Text = "Test Time: " + totalTestTime;
        }

        // ---------------------------
        // Pause Button Click Handler
        // ---------------------------
        private void BtnPause_Click(object sender, EventArgs e)
        {
            // Toggle pause state
            isPaused = !isPaused;
            btnPause.Text = isPaused ? "Resume" : "Pause";
            if (isPaused)
            {
                testStopwatch.Stop();
                AppendLog("Test paused.");
            }
            else
            {
                testStopwatch.Start();
                AppendLog("Test resumed.");
            }
        }

        // ---------------------------
        // Stop Button Click Handler
        // ---------------------------
        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (testCts != null)
            {
                testCts.Cancel();
                AppendLog("Test stop requested.");
                // Reset stopwatch and timer display
                testStopwatch.Reset();
                lblTestTime.Text = "Test Time: 00:00:00";
                uiTimer.Stop();
            }
        }
    }
}
