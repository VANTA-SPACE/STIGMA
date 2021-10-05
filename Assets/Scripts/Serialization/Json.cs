using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Manager;
using UnityEngine;

namespace Serialization {
	public static class Json {
		public static object Deserialize(string json) {
			if (json == null) {
				return null;
			}

			return Parser.Parse(json);
		}

		public static string Serialize(object obj, bool ensureAscii = true) {
			return Serializer.Serialize(obj, ensureAscii);
		}

		private sealed class Parser : IDisposable {
			private Parser(string jsonString) {
				json = new StringReader(jsonString);
			}

			public static object Parse(string jsonString) {
				object result;
				using (Parser parser = new Parser(jsonString)) {
					result = parser.ParseValue();
				}

				return result;
			}

			public void Dispose() {
				json.Dispose();
				json = null;
			}

			private Dictionary<string, object> ParseObject() {
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				json.Read();
				for (;;) {
					TOKEN nextToken = NextToken;
					if (nextToken == TOKEN.NONE) {
						break;
					}

					if (nextToken == TOKEN.CURLY_CLOSE) {
						return dictionary;
					}

					if (nextToken != TOKEN.COMMA) {
						string text = (string) ParseString(true);
						if (text == null) {
							goto NULL;
						}

						if (NextToken != TOKEN.COLON) {
							goto NULL;
						}

						json.Read();
						dictionary[text] = ParseValue();
					}
				}

				NULL:
				return null;
			}

			private List<object> ParseArray() {
				List<object> list = new List<object>();
				json.Read();
				bool flag = true;
				while (flag) {
					TOKEN nextToken = NextToken;
					if (nextToken == TOKEN.NONE) {
						return null;
					}

					if (nextToken != TOKEN.SQUARED_CLOSE) {
						if (nextToken != TOKEN.COMMA) {
							object item = ParseByToken(nextToken);
							list.Add(item);
						}
					} else {
						flag = false;
					}
				}

				return list;
			}

			private object ParseValue() {
				TOKEN nextToken = NextToken;
				return ParseByToken(nextToken);
			}

			private object ParseByToken(TOKEN token) {
				switch (token) {
					case TOKEN.CURLY_OPEN:
						return ParseObject();
					case TOKEN.SQUARED_OPEN:
						return ParseArray();
					case TOKEN.STRING:
						return ParseString(false);
					case TOKEN.NUMBER:
						return ParseNumber();
					case TOKEN.TRUE:
						return true;
					case TOKEN.FALSE:
						return false;
					case TOKEN.NULL:
						return null;
				}

				return null;
			}

			private object ParseString(bool key = true) {
				StringBuilder stringBuilder = new StringBuilder();
				json.Read();
				bool flag = true;
				while (flag) {
					if (json.Peek() == -1) {
						break;
					}

					char nextChar = NextChar;
					if (nextChar != '"') {
						if (nextChar != '\\') {
							stringBuilder.Append(nextChar);
						} else if (json.Peek() == -1) {
							flag = false;
						} else {
							nextChar = NextChar;
							if (nextChar <= '\\') {
								if (nextChar == '"' || nextChar == '/' || nextChar == '\\') {
									stringBuilder.Append(nextChar);
								}
							} else if (nextChar <= 'f') {
								if (nextChar != 'b') {
									if (nextChar == 'f') {
										stringBuilder.Append('\f');
									}
								} else {
									stringBuilder.Append('\b');
								}
							} else if (nextChar != 'n') {
								switch (nextChar) {
									case 'r':
										stringBuilder.Append('\r');
										break;
									case 't':
										stringBuilder.Append('\t');
										break;
									case 'u': {
										StringBuilder stringBuilder2 = new StringBuilder();
										for (int i = 0; i < 4; i++) {
											stringBuilder2.Append(NextChar);
										}

										stringBuilder.Append((char) Convert.ToInt32(stringBuilder2.ToString(), 16));
										break;
									}
								}
							} else {
								stringBuilder.Append('\n');
							}
						}
					} else {
						flag = false;
					}
				}

				var result = stringBuilder.ToString();

				if (!key && result.StartsWith("$") && result.EndsWith("$")) {
					var toParse = result.Substring(1, result.Length - 2);
					var index = toParse.IndexOf("(", StringComparison.Ordinal);
					var name = toParse.Substring(0, index);
					var args = toParse.Substring(index);
					var argList = args.Substring(1, args.Length - 2).Split(',').Select(s => s.Trim()).ToArray();
					switch (name) {
						case "COLOR":
							Color32 color;
							if (argList.Length == 3) {
								color = new Color32(Convert.ToByte(argList[0]), Convert.ToByte(argList[1]), Convert.ToByte(argList[2]), 255);
							} else {
								color = new Color32(Convert.ToByte(argList[0]), Convert.ToByte(argList[1]), Convert.ToByte(argList[2]), Convert.ToByte(argList[3]));
							}

							return color;
						case "ENUM":
							Type type;
							var typeName = argList[0].Split(':')[0];
							var value = argList[0].Split(':')[1];
							if (argList.Length == 2) {
								var assembly = AppDomain.CurrentDomain.GetAssemblies()
									.FirstOrDefault(a => a.GetName().Name == argList[1]);
								if (assembly == null) {
									throw new ArgumentException($"Cannot find assembly {argList[1]}");
								}

								type = assembly.GetType(typeName);
								if (type == null) {
									type = assembly.GetTypes().FirstOrDefault(t =>
										t.IsSubclassOf(typeof(Enum)) &&
										(t.Name == typeName || t.Name.EndsWith("." + typeName)));
								}
							} else {
								type = Type.GetType(typeName);
								if (type == null) {
									type = Assembly.GetAssembly(typeof(GameManager)).GetTypes()
										.FirstOrDefault(t =>
											t.IsSubclassOf(typeof(Enum)) &&
											(t.Name == typeName || t.Name.EndsWith("." + typeName)));
								}
							}

							if (type == null) {
								throw new ArgumentException($"Cannot find type {typeName}");
							}

							return Enum.Parse(type, value);
						default:
							throw new ArgumentException($"Cannot parse ${name}$");
					}
				}
				
				return result;
			}

			private object ParseNumber() {
				string nextWord = NextWord;
				if (nextWord.IndexOf('.') == -1) {
					if (int.TryParse(nextWord, out int num)) return num;
					if (long.TryParse(nextWord, out long lnum)) return num;
					return 0;
				}

				var decLength = nextWord.Split('.')[1].Length;
				if (decLength <= 6) {
					if (float.TryParse(nextWord, out float fnum)) return fnum;
					if (double.TryParse(nextWord, out double dnum)) return dnum;
					return 0;
				}

				if (decLength <= 15) {
					if (double.TryParse(nextWord, out double dnum)) return dnum;
					return 0;
				}
				
				if (decimal.TryParse(nextWord, out decimal mnum)) return mnum;
				if (double.TryParse(nextWord, out double dnum2)) return dnum2;
				return 0;
			}

			private void EatWhitespace() {
				while (" \t\n\r".IndexOf(PeekChar) != -1) {
					json.Read();
					if (json.Peek() == -1) {
						break;
					}
				}
			}

			// (get) Token: 0x0600316B RID: 12651 RVA: 0x00089C4F File Offset: 0x00087E4F
			private char PeekChar {
				get { return Convert.ToChar(json.Peek()); }
			}

			// (get) Token: 0x0600316C RID: 12652 RVA: 0x00089C61 File Offset: 0x00087E61
			private char NextChar {
				get { return Convert.ToChar(json.Read()); }
			}

			// (get) Token: 0x0600316D RID: 12653 RVA: 0x00089C74 File Offset: 0x00087E74
			private string NextWord {
				get {
					StringBuilder stringBuilder = new StringBuilder();
					while (" \t\n\r{}[],:\"".IndexOf(PeekChar) == -1) {
						stringBuilder.Append(NextChar);
						if (json.Peek() == -1) {
							break;
						}
					}

					return stringBuilder.ToString();
				}
			}

			// (get) Token: 0x0600316E RID: 12654 RVA: 0x00089CC0 File Offset: 0x00087EC0
			private TOKEN NextToken {
				get {
					EatWhitespace();
					if (json.Peek() == -1) {
						return TOKEN.NONE;
					}

					char peekChar = PeekChar;
					if (peekChar <= '[') {
						switch (peekChar) {
							case '"':
								return TOKEN.STRING;
							case '#':
							case '$':
							case '%':
							case '&':
							case '\'':
							case '(':
							case ')':
							case '*':
							case '+':
							case '.':
							case '/':
								break;
							case ',':
								json.Read();
								return TOKEN.COMMA;
							case '-':
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								return TOKEN.NUMBER;
							case ':':
								return TOKEN.COLON;
							default:
								if (peekChar == '[') {
									return TOKEN.SQUARED_OPEN;
								}

								break;
						}
					} else {
						if (peekChar == ']') {
							json.Read();
							return TOKEN.SQUARED_CLOSE;
						}

						if (peekChar == '{') {
							return TOKEN.CURLY_OPEN;
						}

						if (peekChar == '}') {
							json.Read();
							return TOKEN.CURLY_CLOSE;
						}
					}

					string nextWord = NextWord;
					if (nextWord == "false") {
						return TOKEN.FALSE;
					}

					if (nextWord == "true") {
						return TOKEN.TRUE;
					}

					if (!(nextWord == "null")) {
						return TOKEN.NONE;
					}

					return TOKEN.NULL;
				}
			}

			private const string WHITE_SPACE = " \t\n\r";

			private const string WORD_BREAK = " \t\n\r{}[],:\"";

			private StringReader json;

			private enum TOKEN {
				NONE,
				CURLY_OPEN,
				CURLY_CLOSE,
				SQUARED_OPEN,
				SQUARED_CLOSE,
				COLON,
				COMMA,
				STRING,
				NUMBER,
				TRUE,
				FALSE,
				NULL
			}
		}

		private sealed class Serializer {
			private const int INDENT = 4;
			private int _currIndent = 0;
			internal bool EnsureAscii = true;

			private void Indent(int indent = INDENT) {
				_currIndent += indent;
			}
			private void UnIndent(int indent = INDENT) {
				_currIndent -= indent;
			}
			
			private Serializer() {
				builder = new StringBuilder();
			}

			private void AppendBuilder(string value) {
				value = value.Replace("\n", "\n" + new string(' ', _currIndent));
				builder.Append(value);
			}
			
			private void AppendBuilder(char value) {
				AppendBuilder(value.ToString());
			}


			public static string Serialize(object obj, bool ensureAscii = true) {
				Serializer serializer = new Serializer();
				serializer.EnsureAscii = ensureAscii;
				serializer.SerializeValue(obj);
				return serializer.builder.ToString();
			}

			private void SerializeValue(object value) {
				if (value == null) {
					AppendBuilder("null");
					return;
				}

				string str;
				if ((str = (value as string)) != null) {
					SerializeString(str);
					return;
				}

				if (value is bool) {
					AppendBuilder(value.ToString().ToLower());
					return;
				}

				IList anArray;
				if ((anArray = (value as IList)) != null) {
					SerializeArray(anArray);
					return;
				}

				IDictionary obj;
				if ((obj = (value as IDictionary)) != null) {
					SerializeObject(obj);
					return;
				}

				if (value is char) {
					SerializeString(value.ToString());
					return;
				}

				SerializeOther(value);
			}

			private void SerializeObject(IDictionary obj) {
				foreach (var value in obj.Values) {
					if (value is IDictionary dictionary && dictionary.Count > 0 ||
					    value is IList list && list.Count > 0) {
						SerializeObjectWithNL(obj);
						return;
					}
				}
				
				if (obj.Keys.Count > 0) {
					bool flag = true;
					AppendBuilder("{");
					foreach (object obj2 in obj.Keys) {
						if (!flag) {
							AppendBuilder(", ");
						}

						SerializeString(obj2.ToString());
						AppendBuilder(": ");
						SerializeValue(obj[obj2]);
						flag = false;
					}
					AppendBuilder("}");
				} else AppendBuilder("{ }");

			}
			private void SerializeArray(IList anArray) {

				foreach (var value in anArray) {
					if (value is IDictionary dictionary && dictionary.Count > 0 ||
					    value is IList list && list.Count > 0) {
						SerializeArrayWithNL(anArray);
						return;
					}
				}

				Indent();
				AppendBuilder('[');
				bool flag = true;
				if (anArray.Count > 0) {
					foreach (object value in anArray) {
						if (!flag) {
							AppendBuilder(", ");
						}

						SerializeValue(value);
						flag = false;
					}
				} else {
					AppendBuilder(' ');
				}

				UnIndent();
				AppendBuilder(']');
			}
			
			private void SerializeArrayWithNL(IList anArray) {
				Indent();
				AppendBuilder("[\n");
				bool flag = true;
				if (anArray.Count > 0) {
					foreach (object value in anArray) {
						if (!flag) {
							AppendBuilder(", \n");
						}

						SerializeValue(value);
						flag = false;
					}
				} else {
					AppendBuilder(' ');
				}

				UnIndent();
				AppendBuilder("\n]");
			}
			
			
			private void SerializeObjectWithNL(IDictionary obj) {
				if (obj.Keys.Count > 0) {
					Indent();
					bool flag = true;
					AppendBuilder("{\n");
					foreach (object obj2 in obj.Keys) {
						if (!flag) {
							AppendBuilder(",\n");
						}

						SerializeString(obj2.ToString());
						AppendBuilder(": ");
						SerializeValue(obj[obj2]);
						flag = false;
					}
					UnIndent();
					AppendBuilder("\n}");
				} else AppendBuilder("{ }");

			}

			private void SerializeString(string str) {
				AppendBuilder('"');
				char[] array = str.ToCharArray();
				int i = 0;
				while (i < array.Length) {
					char c = array[i];
					switch (c) {
						case '\b':
							AppendBuilder("\\b");
							break;
						case '\t':
							AppendBuilder("\\t");
							break;
						case '\n':
							AppendBuilder("\\n");
							break;
						case '\v':
							goto END;
						case '\f':
							AppendBuilder("\\f");
							break;
						case '\r':
							AppendBuilder("\\r");
							break;
						default:
							if (c != '"') {
								if (c != '\\') {
									goto END;
								}

								AppendBuilder("\\\\");
							} else {
								AppendBuilder("\\\"");
							}

							break;
					}

					LOOP:
					i++;
					continue;
					END:
					int num = Convert.ToInt32(c);
					if (num >= 32 && num <= 126 || !EnsureAscii) {
						AppendBuilder(c);
						goto LOOP;
					}

					AppendBuilder("\\u" + Convert.ToString(num, 16).PadLeft(4, '0'));
					goto LOOP;
				}

				AppendBuilder('"');
			}

			private void SerializeOther(object value) {
				if (value is float || value is double || value is decimal) {
					var num = value.ToString();
					if (!num.Contains(".")) {
						num += ".0";
					}
					AppendBuilder(num);
					return;
				}

				if (value is int || value is uint || value is long || value is sbyte || value is byte ||
				    value is short || value is ushort || value is ulong) {
					AppendBuilder(value.ToString());
					return;
				}

				if (value is Color c) {
					var color = (Color32) c;
					SerializeString($"$COLOR({color.r}, {color.g}, {color.b}, {color.a})$");
					return;
				}
				
				if (value is Color32 color32) {
					SerializeString($"$COLOR({color32.r}, {color32.g}, {color32.b}, {color32.a})$");
					return;
				}
				
				if (value is Enum enumValue) {
					var type = enumValue.GetType();
					if (type.Assembly == Assembly.GetAssembly(typeof(GameManager)))
						SerializeString($"$ENUM({type.Name}:{value})$");
					else 
						SerializeString($"$ENUM({type.Namespace}.{type.Name}:{value}, {type.Assembly.GetName().Name})$");
					return;
				}

				SerializeString(value.ToString());
			}

			private StringBuilder builder;
		}
	}
}