using System.Runtime.InteropServices;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ImageFinderNS;
using OpenQA.Selenium.Chrome;
using WindowsInput.Native;
using WindowsInput;

namespace Macro
{
    public partial class Form1 : Form
    {
        #region Settings
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public const int VK_S = 0x53;

        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const int MOUSEEVENTF_LEFTUP = 0x0004;

        public enum WMessages : int
        {
            Key_S = 0x53
        }

        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        #endregion

        public Form1()
        {
            InitializeComponent();
            timeStamp("KMacro Version 1.0");
        }

        public void WebdriverInput(string text, IWebDriver driver)
        {
            Actions actionProvider = new Actions(driver);
            Clipboard.SetText(text);

            IAction keydown = actionProvider.KeyDown(OpenQA.Selenium.Keys.Control).SendKeys("v").KeyUp(OpenQA.Selenium.Keys.Control).Build();
            keydown.Perform();
        }

        public object GetID()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            String spreadsheetId = "";
            String range = "test!A1:B9";
            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            if (values != null && values.Count > 0)
            {
                int num = 0;
                var row = values[num];
                // row = values[num + 1];

                return row;
            }
            else
            {
                Console.WriteLine("No data Found.");
            }

            return null;
        }

        public async void Web_Start()
        {
            // ChromeDriver Settings
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeDriver chromeDriver = new ChromeDriver(chromeDriverService);

            // Login Urls
            string login_NEXON = "https://nxlogin.nexon.com/common/login.aspx?redirect=https%3A%2F%2Ffifaonline4.nexon.com%2Fmain%2Findex";
            string login_NAVER = "https://nid.naver.com/oauth2.0/authorize?response_type=code&svctype=0&client_id=9tEsmZTa1qdnr6eIz2yO&redirect_uri=https://login.nexon.com/login/naver/AccessToken&state=O_lllTE0CCfHxfcCIhQM9DaRnPs6GeEFcsD2XhZSD_EUbjtnkfU9dUspWxvzj_9UxGcJ~J6y_CWakqiUmad5szR0S_XrRpdN2kDlxGc6YuabPqHAAnrsf_b0JXTGJgvq3CeaYKBZoesqfHiN~jDnA2M00kiv32ID";
            string login_GOOGLE = "https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?scope=profile%20email&response_type=code&client_id=919331056041-46d8sbblkb1ek3o02iva7vaiqth50clq.apps.googleusercontent.com&redirect_uri=https%3A%2F%2Flogin.nexon.com%2Flogin%2Fgoogle%2FAccessToken&state=6t3joCGJGzrFRNiUyx8H~yP1nmNcpKd3vDUG2~uoDdf0AIlEpgA~xAkzn80tO2WQPSWW~V8baIsXeunPO3z4Z4BE59tdJeUnY0abjGf7iTZ8FIRuq4oGWa~SQYiHIXyR~gK6iixjUPkI6RQLshicVCC218YPd_b8&include_granted_scopes=true&access_type=offline&flowName=GeneralOAuthFlow";
            string login_FACEBOOK = "https://www.facebook.com/login.php?skip_api_login=1&api_key=198868903993596&kid_directed_site=0&app_id=198868903993596&signed_next=1&next=https%3A%2F%2Fwww.facebook.com%2Fdialog%2Foauth%3Fscope%3Demail%26client_id%3D198868903993596%26redirect_uri%3Dhttps%253A%252F%252Flogin.nexon.com%252Flogin%252Ffacebook%252FAccessToken%26state%3DcaAIKw_x88VWlVkeGEILxKFKa6FVZA6k_DZ1WVrjLMELfrsQGVyPuKxLET9I0tQdJ8TQ3vN4_x4jSjR04GYPtBMHeezH3J3_Xjtl1ANHyNaskDCygmxYg6XxinbYks7BLqWjOT3Q4KV9dCMTOT0VPXiAuuqP9Rx%257E%26ret%3Dlogin%26fbapp_pres%3D0%26logger_id%3D3b7275fe-6325-46c5-9781-d18ce8eda43c%26tp%3Dunspecified&cancel_url=https%3A%2F%2Flogin.nexon.com%2Flogin%2Ffacebook%2FAccessToken%3Ferror%3Daccess_denied%26error_code%3D200%26error_description%3DPermissions%2Berror%26error_reason%3Duser_denied%26state%3DcaAIKw_x88VWlVkeGEILxKFKa6FVZA6k_DZ1WVrjLMELfrsQGVyPuKxLET9I0tQdJ8TQ3vN4_x4jSjR04GYPtBMHeezH3J3_Xjtl1ANHyNaskDCygmxYg6XxinbYks7BLqWjOT3Q4KV9dCMTOT0VPXiAuuqP9Rx%257E%23_%3D_&display=page&locale=ko_KR&pl_dbl=0";

            // Get Member's ID Type from Spread Sheet
            string idType = "넥슨";

            timeStamp("회원 아이디 구분 ( " + idType + " )");

            switch (idType)
            {
                #region NEXON LOGIN
                case "넥슨":
                    // Connect to Login Page
                    try
                    {
                        timeStamp("로그인 페이지 접속");
                        chromeDriver.Navigate().GoToUrl(login_NEXON);
                    }
                    catch
                    {
                        MessageBox.Show("Invalid URL");
                    }

                    timeStamp("회원 아이디 기입");
                    // Get Web Components       
                    var Field_ID_NEXON = chromeDriver.FindElement(By.Id("txtNexonID"));
                    var Field_PW_NEXON = chromeDriver.FindElement(By.Id("txtPWD"));
                    var Button_LOGIN_NEXON = chromeDriver.FindElement(By.ClassName("button01"));

                    // Member Data from Spread Sheet
                    var info = (IList<Object>)GetID();
                    Field_ID_NEXON.SendKeys(Convert.ToString(info[0]));
                    Field_PW_NEXON.SendKeys(Convert.ToString(info[1]));

                    // Login Action
                    try
                    {
                        timeStamp("로그인 시도");
                        Button_LOGIN_NEXON.Click();
                    }
                    catch
                    {
                        MessageBox.Show("Login Click Failed");
                    }

                    break;
                #endregion

                #region NAVER LOGIN
                case "네이버":
                    // Connect to Login Page
                    chromeDriver.Navigate().GoToUrl(login_NAVER);

                    // Get Web Components
                    // Member Data from Spread Sheet
                    chromeDriver.FindElement(By.Id("id")).Click();
                    WebdriverInput("", chromeDriver);

                    await Task.Delay(1000);

                    chromeDriver.FindElement(By.Id("pw")).Click();
                    WebdriverInput("", chromeDriver);

                    var Button_LOGIN_NAVER = chromeDriver.FindElement(By.ClassName("btn_login"));

                    // Login Action
                    Button_LOGIN_NAVER.Click();

                    await Task.Delay(1000);

                    chromeDriver.FindElement(By.ClassName("btn_unit_on")).Click();
                    break;
                #endregion 

                // Web Components are hashed (Auto-Login is Impossible)
                #region GOOGLE LOGIN
                case "구글":
                    // Connect to Login Page
                    chromeDriver.Navigate().GoToUrl(login_GOOGLE);

                    // Get Web Components
                    var Field_ID_GOOGLE = chromeDriver.FindElement(By.Id("identifierId"));
                    var Field_PW_GOOGLE = chromeDriver.FindElement(By.Id("txtPWD"));
                    var Button_LOGIN_GOOGLE = chromeDriver.FindElement(By.ClassName("button01"));

                    // Member Data from Spread Sheet
                    Field_ID_GOOGLE.SendKeys("");
                    Field_PW_GOOGLE.SendKeys("");

                    // Login Action
                    Button_LOGIN_GOOGLE.Click();
                    break;
                #endregion

                // Agree Button is existed (Auto-Login is Difficult)
                #region FACEBOOK LOGIN
                case "페이스북":
                    // Connect to Login Page
                    chromeDriver.Navigate().GoToUrl(login_FACEBOOK);

                    // Get Web Components
                    var Field_ID_FACEBOOK = chromeDriver.FindElement(By.Id("email"));
                    var Field_PW_FACEBOOK = chromeDriver.FindElement(By.Id("pass"));
                    var Button_LOGIN_FACEBOOK = chromeDriver.FindElement(By.Id("loginbutton"));

                    // Member Data from Spread Sheet
                    Field_ID_FACEBOOK.SendKeys("");
                    Field_PW_FACEBOOK.SendKeys("");

                    // Login Action
                    Button_LOGIN_FACEBOOK.Click();
                    break;
                #endregion

                // To Handle Exceptions
                #region DEFAULT CASE
                default:
                    // Should Make Exception Handler
                    break;
                    #endregion
            }

            await Task.Delay(5000);
            var Popup_Close = chromeDriver.FindElements(By.ClassName("btn_close"));

            for (int i = 0; i < 2; i++)
            {
                Popup_Close[i].Click();
            }

            timeStamp("게임 시작");
            var Game_Start = chromeDriver.FindElement(By.ClassName("btn_gamestart_obt"));
            Game_Start.Click();
        }

        public void Mouse_Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public bool ImageSearch(string location)
        {
            ImageFinder.SetSource(ImageFinder.MakeScreenshot());
            var finds = ImageFinder.Find(Image.FromFile(location), 1.0f);

            foreach (var find in finds)
            {
                if (find.Similarity > 0.5f)
                {
                    Rectangle rect = find.Zone;
                    Cursor.Position = new System.Drawing.Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                    Mouse_Click();

                    return true;
                }
            }

            return false;
        }

        public void timeStamp(string param)
        {
            string nowTime = DateTime.Now.ToString("HH:mm:ss");
            systemMsg.AppendText(nowTime + " :: " + param + "\n");
            systemMsg.ScrollToCaret();
        }

        private async void Start_Click(object sender, EventArgs e)
        {
            
            bool flag_OnGame = false;
            bool flag_GongjiX = false;
            bool flag_inclub = false;
            bool flag_inclub_proceed = false;
            bool flag_MGRMode = false;
            /*
            timeStamp("프로그램 시작");
            Web_Start();

            timeStamp("게임 감지 중");
            while (true)
            {
                if (FindWindow(null, "FIFA ONLINE 4") != IntPtr.Zero)
                {
                    timeStamp("게임 감지 성공");
                    flag_OnGame = true;
                    break;
                }
                await Task.Delay(1000);
            }
            
            while (!flag_GongjiX)
            {
                if (ImageSearch(@"img\GongjiX.png"))
                {
                    flag_GongjiX = true;
                    timeStamp("매크로 동작 - 공지 끄기");
                    break;
                }
                await Task.Delay(1000);
            }*/

            while (!flag_inclub)
            {
                if (ImageSearch(@"img\Proceed2.png"))
                {
                    flag_inclub = true;
                    timeStamp("매크로 동작 - 일정 진행 클릭");
                    break;
                }
                await Task.Delay(1000);
            }

            while (!flag_inclub_proceed)
            {
                if (ImageSearch(@"img\Proceed.png"))
                {
                    flag_inclub_proceed = true;
                    timeStamp("매크로 동작 - 진행 버튼 클릭");
                    break;
                }
                await Task.Delay(1000);
            }

            while (!flag_MGRMode)
            {
                if (ImageSearch(@"img\MGRMode.png"))
                {
                    flag_MGRMode = true;
                    timeStamp("매크로 동작 - 감독 모드 클릭");
                    break;
                }
                await Task.Delay(1000);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
