using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coupang_partnerts_api
{
    public partial class Form1 : Form
    {
        private static readonly String REQUEST_METHOD = "GET";
        private static readonly String DOMAIN = "https://api-gateway.coupang.com";
        private String URL = "";
        private String ACCESS_KEY = "";
        private String SECRET_KEY = "";
        private static readonly String _SubID = "";

        private static readonly String REQUEST_JSON = @"{
            ""coupangUrls"": [
                ""https://www.coupang.com/np/search?component=&q=good&channel=user"",
                ""https://www.coupang.com/np/coupangglobal""
            ]
        }";
        public Form1()
        {
            InitializeComponent();
            InitializeComboBox();
            InitializeKey();
        }
        private void InitializeKey() 
        {
            string filePath = "keydata.txt";
            try
            {

                if (!File.Exists(filePath))
                {
                    string[] defaultKeys = new string[] { "defaultAccessKey", "defaultSecretKey" };
                    File.WriteAllLines(filePath, defaultKeys);
                    MessageBox.Show("keydata.txt 파일이 생성되었습니다. 기본 키 값을 확인하고 수정해주세요.");
                }

                string[] lines = File.ReadAllLines(filePath);

                // 첫 번째 줄을 ACCESS_KEY로, 두 번째 줄을 SECRET_KEY로 설정
                foreach (string line in lines)
                {
                    if (line.StartsWith("AccessKey :"))
                    {
                        ACCESS_KEY = line.Substring("AccessKey :".Length).Trim();
                        textBox2.Text = ACCESS_KEY;
                    }
                    else if (line.StartsWith("SecretKey :"))
                    {
                        SECRET_KEY = line.Substring("SecretKey :".Length).Trim();
                        textBox3.Text = SECRET_KEY;
                    }
                }

                // 키 값이 제대로 설정되었는지 확인
                if (string.IsNullOrEmpty(ACCESS_KEY) || string.IsNullOrEmpty(SECRET_KEY))
                {
                    MessageBox.Show("keydata.txt 파일에 필요한 키 값이 부족합니다.");
                }
                else
                {
                    MessageBox.Show("키 값이 성공적으로 설정되었습니다.");
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine("키를 초기화하는 중 오류가 발생했습니다: " + ex.Message);
                MessageBox.Show("키를 초기화하는 중 오류가 발생했습니다: " + ex.Message);
            }
        }
        private void InitializeComboBox()
        {
            var items = new List<CategoryItem>
            {
                new CategoryItem("여성패션", 1001),
                new CategoryItem("남성패션", 1002),
                new CategoryItem("뷰티", 1010),
                new CategoryItem("출산/유아동", 1011),
                new CategoryItem("식품", 1012),
                new CategoryItem("주방용품", 1013),
                new CategoryItem("생활용품", 1014),
                new CategoryItem("홈인테리어", 1015),
                new CategoryItem("가전디지털", 1016),
                new CategoryItem("스포츠/레저", 1017),
                new CategoryItem("자동차용품", 1018),
                new CategoryItem("도서/음반/DVD", 1019),
                new CategoryItem("완구/취미", 1020),
                new CategoryItem("문구/오피스", 1021),
                new CategoryItem("헬스/건강식품", 1024),
                new CategoryItem("국내여행", 1025),
                new CategoryItem("해외여행", 1026),
                new CategoryItem("반려동물용품", 1029),
                new CategoryItem("유아동패션", 1030)
            };

            comboBox1.DataSource = items;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private bool keyCheck()
        {
  
            if (string.IsNullOrEmpty(ACCESS_KEY) || string.IsNullOrEmpty(SECRET_KEY))
            {
                return false;
            }

            return true;

        }

         public class CategoryItem
        {
            public string Name { get; set; }
            public int Value { get; set; }

            public CategoryItem(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem is CategoryItem selectedItem)
            {
                URL = $"/v2/providers/affiliate_open_api/apis/openapi/products/bestcategories/{selectedItem.Value}?limit=50";
                MessageBox.Show($"선택된 카테고리: {selectedItem.Name}\nURL 값이 업데이트 되었습니다: {URL}");
            }

        }

        private async Task<string> SendRequestAsync(string url)
        {
            if (keyCheck() == true) {

                var signature = HmacGenerator.GenerateHmac(REQUEST_METHOD, url, ACCESS_KEY, SECRET_KEY);
                var apiUrl = $"{DOMAIN}{url}";
                var request = WebRequest.CreateHttp(apiUrl);
                request.Method = REQUEST_METHOD;
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", signature);

                try
                {

                    using (var response = await request.GetResponseAsync() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            return await reader.ReadToEndAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
            return "Not setting key";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var responseText = await SendRequestAsync(URL);
            textBox1.Text = responseText;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
