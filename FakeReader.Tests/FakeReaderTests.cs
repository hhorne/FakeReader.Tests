using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Xunit;

namespace FakeReader.Tests
{
    public class FakeXsltReaderTests
    {
	    public class OpenMethod
	    {
			[Fact]
			public void ReturnsTrueWhenCsvFileNameMatchesIndex()
			{
				var reader = new FakeXsltReader();
				bool result = reader.Open("index");
				Assert.True(result);
			}

		    [Fact]
		    public void ReturnsFalseWhenIndexNameCantBeMatchedToCsvFile()
			{
				var reader = new FakeXsltReader();
				bool result = reader.Open("foo");
				Assert.False(result);
		    }

			[Fact]
			public void GetFieldValueReturnsStringWhenColumnExists()
			{
				var reader = new FakeXsltReader();
				reader.Open("index");
				reader.Read();

				var val = reader.GetFieldValue("batch_id");

				Assert.NotEqual(string.Empty, val);
			}
	    }

		public class ReadMethod
		{
			[Fact]
			public void ReturnsFalseWhenNoRecordsPresent()
			{
				var reader = new FakeXsltReader();
				reader.Open("emptyindex");

				bool result = reader.Read();

				Assert.False(result);
			}

			[Fact]
			public void ReturnsTrueWhenRecordsArePresent()
			{
				var reader = new FakeXsltReader();
				reader.Open("index");

				bool result = reader.Read();

				Assert.True(result);
			}
		}

		public class GetFieldValueMethod
		{
			[Fact]
			public void ReturnsNullWhenFieldDoesntExist()
			{			
				var reader = new FakeXsltReader();
				reader.Open("index");
				reader.Read();

				var result = reader.GetFieldValue("foo");
				
				Assert.Null(result);
			}

			[Fact]
			public void ReturnsStringWhenFieldExists()
			{
				var reader = new FakeXsltReader();
				reader.Open("index");
				reader.Read();

				var result = reader.GetFieldValue("batch_id");

				Assert.NotNull(result);
				Assert.IsType<string>(result);
			}
		}
    }

	public class FakeXsltReader : IXsltReader
	{
		CsvReader _CsvReader;
		CsvConfiguration _CsvConfiguration;
		Dictionary<string, int> _Counters; 

		public FakeXsltReader()
		{
			_Counters = new Dictionary<string, int>();
			_CsvConfiguration = new CsvConfiguration();
			_CsvConfiguration.IsStrictMode = false;
		}

		public bool Open(string index)
		{
			try
			{
				string fileName = index + ".csv";

				if (!File.Exists(fileName))
					return false;

				_CsvConfiguration.HasHeaderRecord = true;
				_CsvReader = new CsvReader(new StreamReader(fileName), _CsvConfiguration);
				return true;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public bool Read()
		{
			return _CsvReader.Read();
		}

		public string GetFieldValue(string fieldName)
		{
			return _CsvReader.GetField(fieldName);
		}
	}

	public interface IXsltReader
	{
		bool Open(string index);
		bool Read();
		string GetFieldValue(string fieldName);
	}
}
