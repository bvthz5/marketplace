using System.Xml;

namespace MarketPlaceUserTest.Xml
{
    public class SonarlintTest
    {
        [Theory]
        [InlineData(@"<AnalysisInput xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><Settings><Setting><Key>sonar.cs.analyzeGeneratedCode</Key><Value>false</Value></Setting><Setting><Key>sonar.cs.file.suffixes</Key><Value>.cs</Value></Setting><Setting><Key>sonar.cs.ignoreHeaderComments</Key><Value>true</Value></Setting><Setting><Key>sonar.cs.roslyn.ignoreIssues</Key><Value>false</Value></Setting></Settings><Rules><Rule><Key>S107</Key><Parameters><Parameter><Key>max</Key><Value>7</Value></Parameter></Parameters></Rule><Rule><Key>S110</Key><Parameters><Parameter><Key>max</Key><Value>5</Value></Parameter></Parameters></Rule><Rule><Key>S1479</Key><Parameters><Parameter><Key>maximum</Key><Value>30</Value></Parameter></Parameters></Rule><Rule><Key>S2342</Key><Parameters><Parameter><Key>flagsAttributeFormat</Key><Value>^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?s$</Value></Parameter><Parameter><Key>format</Key><Value>^([A-Z]{1,3}[a-z0-9]+)*([A-Z]{2})?$</Value></Parameter></Parameters></Rule><Rule><Key>S2436</Key><Parameters><Parameter><Key>max</Key><Value>2</Value></Parameter><Parameter><Key>maxMethod</Key><Value>3</Value></Parameter></Parameters></Rule><Rule><Key>S3776</Key><Parameters><Parameter><Key>propertyThreshold</Key><Value>3</Value></Parameter><Parameter><Key>threshold</Key><Value>15</Value></Parameter></Parameters></Rule></Rules></AnalysisInput>")]
        public void AnalysisInput_WithValidXml_ShouldParseSuccessfully(string xml)
        {
            // Arrange
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            // Act
            var settings = doc.SelectNodes("//Settings/Setting");
            var rules = doc.SelectNodes("//Rules/Rule");

            // Assert
            Assert.NotNull(settings);
            Assert.NotNull(rules);
            Assert.Equal(4, settings.Count);
            Assert.Equal(6, rules.Count);
        }
    }
}
