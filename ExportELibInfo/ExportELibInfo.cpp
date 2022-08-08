#include "stdafx.h"
#include "3rd-party/elib/lib2.h"
#include "3rd-party/elib/lang.h"
#include "3rd-party/rapidjson/include/rapidjson/writer.h"
#include <iostream>
#include <string>
using namespace rapidjson;

template<typename OutputStream, typename TargetEncoding, typename StackAllocator, unsigned writeFlags>
void AnsiString(Writer<OutputStream, UTF16LE<>, TargetEncoding, StackAllocator, writeFlags>& writer, LPSTR str, int codepage) {
	if (str == nullptr)
	{
		writer.Null();
		return;
	}
	int size = MultiByteToWideChar(codepage, 0, str, -1, nullptr, 0);
	auto wstr = new wchar_t[size];
	MultiByteToWideChar(codepage, 0, str, -1, wstr, size);
	writer.String(wstr, size - 1, true);
	delete[] wstr;
}

template<typename OutputStream, typename TargetEncoding, typename StackAllocator, unsigned writeFlags>
void WriteVersion(Writer<OutputStream, UTF16LE<>, TargetEncoding, StackAllocator, writeFlags>& writer, int major, int minor) {
	wchar_t versionStr[32];
	swprintf_s(versionStr, L"%d.%d", major, minor);
	writer.String(versionStr);
}

template<typename OutputStream, typename TargetEncoding, typename StackAllocator, unsigned writeFlags>
void WriteDefaultKVPair(Writer<OutputStream, UTF16LE<>, TargetEncoding, StackAllocator, writeFlags>& writer, DATA_TYPE dataType, INT_PTR value, int codepage) {
	switch (dataType)
	{
	case SDT_BYTE:
	case SDT_SHORT:
	case SDT_INT:
	case SDT_INT64:
		writer.Key(L"Default");
		writer.Int(value);
		break;
	case SDT_FLOAT:
	case SDT_DOUBLE:
		writer.Key(L"Default");
		writer.Double((double)value);
		break;
	case SDT_BOOL:
		writer.Key(L"Default");
		writer.Bool(value != 0);
		break;
	case SDT_TEXT:
		writer.Key(L"Default");
		AnsiString(writer, (LPSTR)value, codepage);
		break;
	default:
		break;
	}
}

DATA_TYPE UnitPropertyTypeToDataType(PLIB_INFO pLibInfo, SHORT shtType)
{
	switch (shtType)
	{
	case UD_PICK_SPEC_INT:
	case UD_INT:
	case UD_PICK_INT:
		return SDT_INT;
	case UD_DOUBLE:
		return SDT_DOUBLE;
	case UD_BOOL:
		return SDT_BOOL;
	case UD_DATE_TIME:
		return SDT_DATE_TIME;
	case UD_TEXT:
	case UD_PICK_TEXT:
	case UD_EDIT_PICK_TEXT:
	case UD_FILE_NAME:
		return SDT_TEXT;
	case UD_PIC:
	case UD_ICON:
	case UD_CURSOR:
	case UD_MUSIC:
	case UD_IMAGE_LIST:
	case UD_CUSTOMIZE:
		return SDT_BIN;
	case UD_COLOR:
	case UD_COLOR_TRANS:
	case UD_COLOR_BACK:
		return SDT_INT;
	case UD_FONT:
		return pLibInfo->m_nDataTypeCount + 4;
	default:
		return 0;
	}
}

int main()
{
	setlocale(LC_ALL, "");

	int nArgs;
	auto szArglist = CommandLineToArgvW(GetCommandLineW(), &nArgs);
	if (nullptr == szArglist) return 1;
	std::wstring fneName;
	HANDLE hFile = 0;

	switch (nArgs)
	{
	case 2:
		fneName = szArglist[1];
		break;
	case 3:
		fneName = szArglist[1];
		hFile = CreateFileW(szArglist[2], GENERIC_WRITE, 0, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
		break;
	default:
		LocalFree(szArglist);
		return 1;
	}
	LocalFree(szArglist);

	auto fne = LoadLibraryW(fneName.c_str());
	if (nullptr == fne)
	{
		wchar_t fnePath[MAX_PATH];
		HKEY hkey;
		if (RegOpenKeyExW(HKEY_CURRENT_USER, L"Software\\FlySky\\E\\Install", 0, KEY_READ, &hkey) == ERROR_SUCCESS)
		{
			DWORD len = sizeof(fnePath);
			if (RegQueryValueExW(hkey, L"Path", nullptr, nullptr, (LPBYTE)fnePath, &len) == ERROR_SUCCESS)
			{
				len = len / sizeof(wchar_t) - 1;
				if (fnePath[len - 1] != '\\')
				{
					wcscat(fnePath, L"\\");
					len += 1;
				}
				auto len_fneName = fneName.length();
				memcpy(fnePath + len, fneName.c_str(), (len_fneName + 1) * sizeof(wchar_t));
				if (fneName.rfind(L".") == std::wstring::npos)
				{
					wcscat(fnePath, L".fne");
				}
				fne = LoadLibraryW(fnePath);
			}
			RegCloseKey(hkey);
		}
	}
	if (nullptr == fne) return 1;
	auto getNewInf = (PFN_GET_LIB_INFO)GetProcAddress(fne, FUNCNAME_GET_LIB_INFO);
	if (nullptr == getNewInf) return 1;
	auto pLibInfo = getNewInf();
	if (pLibInfo->m_dwState & LBS_NO_EDIT_INFO) return 1;
	int codepage;
	switch (pLibInfo->m_nLanguage)
	{
	case __GBK_LANG_VER:
		if (IsValidCodePage(54936))
			codepage = 54936; //GB18030
		else
			codepage = 936; //GBK
		break;
	case __ENGLISH_LANG_VER:
		codepage = 1252; //ASCII
		break;
	case __BIG5_LANG_VER:
		codepage = 950; //Big5
		break;
	case __SJIS_JP_LANG_VER:
		codepage = 932; //Shift-JIS
		break;
	default:
		codepage = 0;
		break;
	}

	GenericStringBuffer<UTF16LE<> >  s;
	Writer<GenericStringBuffer<UTF16LE<> >, UTF16LE<>, UTF16LE<> > writer(s);

	writer.StartObject();
	writer.Key(L"Guid");
	AnsiString(writer, pLibInfo->m_szGuid, codepage);
	writer.Key(L"Name");
	AnsiString(writer, pLibInfo->m_szName, codepage);
	writer.Key(L"FileName");
	{
		auto posOfDot = fneName.rfind(L".");
		if (posOfDot == std::wstring::npos)
		{
			writer.String(fneName.c_str(), fneName.length(), true);
		}
		else
		{
			writer.String(fneName.c_str(), posOfDot, true);
		}
	}
	writer.Key(L"Version");
	WriteVersion(writer, pLibInfo->m_nMajorVersion, pLibInfo->m_nMinorVersion);
	writer.Key(L"VersionCode");
	writer.Int(pLibInfo->m_nBuildNumber);
	writer.Key(L"MinRequiredEplVersion");
	WriteVersion(writer, pLibInfo->m_nRqSysMajorVer, pLibInfo->m_nRqSysMinorVer);
	writer.Key(L"MinRequiredKrnlnVersion");
	WriteVersion(writer, pLibInfo->m_nRqSysKrnlLibMajorVer, pLibInfo->m_nRqSysKrnlLibMinorVer);
	writer.Key(L"Description");
	AnsiString(writer, pLibInfo->m_szExplain, codepage);

	writer.Key(L"Author");
	writer.StartObject();
	writer.Key(L"Name");
	AnsiString(writer, pLibInfo->m_szAuthor, codepage);
	writer.Key(L"ZipCode");
	AnsiString(writer, pLibInfo->m_szZipCode, codepage);
	writer.Key(L"Address");
	AnsiString(writer, pLibInfo->m_szAddress, codepage);
	writer.Key(L"Telephone");
	AnsiString(writer, pLibInfo->m_szPhoto, codepage);
	writer.Key(L"QQ");
	AnsiString(writer, pLibInfo->m_szFax, codepage);
	writer.Key(L"Email");
	AnsiString(writer, pLibInfo->m_szEmail, codepage);
	writer.Key(L"HomePage");
	AnsiString(writer, pLibInfo->m_szHomePage, codepage);
	writer.Key(L"Additional");
	AnsiString(writer, pLibInfo->m_szOther, codepage);
	writer.EndObject();

	writer.Key(L"DataTypes");
	writer.StartArray();
	for (int i = 0; i < pLibInfo->m_nDataTypeCount; i++)
	{
		auto& type = pLibInfo->m_pDataType[i];
		writer.StartObject();
		writer.Key(L"Name");
		AnsiString(writer, type.m_szName, codepage);
		writer.Key(L"EnglshName");
		AnsiString(writer, type.m_szEgName, codepage);
		if (type.m_szExplain)
		{
			writer.Key(L"Description");
			AnsiString(writer, type.m_szExplain, codepage);
		}
		if (type.m_dwState & LDT_IS_HIDED)
		{
			writer.Key(L"Deprecated");
			if (type.m_dwState & LDT_IS_ERROR)
			{
				writer.String(L"Error");
			}
			else
			{
				writer.String(L"Hidden");
			}
		}
		if (type.m_dwState & LDT_ENUM)
		{
			writer.Key(L"Kind");
			writer.String(L"Enum");
		}
		else if (type.m_dwState & LDT_WIN_UNIT)
		{
			writer.Key(L"Kind");
			writer.String(L"Component");
			if (type.m_dwState & LDT_IS_CONTAINER)
			{
				writer.Key(L"IsContainer");
				writer.Bool(true);
			}
			if (type.m_dwState & LDT_IS_TAB_UNIT)
			{
				writer.Key(L"IsTabControl");
				writer.Bool(true);
			}
			if (type.m_dwState & LDT_IS_FUNCTION_PROVIDER)
			{
				writer.Key(L"IsFunctional");
				writer.Bool(true);
			}
			if (type.m_dwState & LDT_CANNOT_GET_FOCUS)
			{
				writer.Key(L"CanFocus");
				writer.Bool(false);
				writer.Key(L"NoTabStopByDefault");
				writer.Bool(true);
			} 
			else if (type.m_dwState & LDT_DEFAULT_NO_TABSTOP)
			{
				writer.Key(L"NoTabStopByDefault");
				writer.Bool(true);
			}
			if (type.m_dwState & LDT_MSG_FILTER_CONTROL)
			{
				writer.Key(L"IsMessageFilter");
				writer.Bool(true);
			}
			if (type.m_dwUnitBmpID)
			{
				writer.Key(L"ImageId");
				writer.Int(type.m_dwUnitBmpID);
			}
		}
		writer.Key(L"Evnets");
		writer.StartArray();
		intptr_t pEvent = reinterpret_cast<intptr_t>(type.m_pEventBegin);
		for (int j = 0; j < type.m_nEventCount; j++)
		{
			auto& event = *reinterpret_cast<EVENT_INFO*>(pEvent);
			writer.StartObject();
			writer.Key(L"Name");
			AnsiString(writer, event.m_szName, codepage);
			if (event.m_szExplain)
			{
				writer.Key(L"Description");
				AnsiString(writer, event.m_szExplain, codepage);
			}
			if (event.m_dwState & EV_IS_HIDED)
			{
				writer.Key(L"Deprecated");
				writer.String(L"Hidden");
			}

			if (event.m_dwState & EV_IS_VER2)
			{
				writer.Key(L"ReturnDataType");
				writer.Int(reinterpret_cast<EVENT_INFO2&>(event).m_dtRetDataType);
			}
			else
			{
				if (event.m_dwState & EV_RETURN_INT)
				{
					writer.Key(L"ReturnDataType");
					writer.Int(SDT_INT);
				}
				else if (event.m_dwState & EV_RETURN_BOOL)
				{
					writer.Key(L"ReturnDataType");
					writer.Int(SDT_BOOL);
				}
			}

			writer.Key(L"Parameters");
			writer.StartArray();
			if (event.m_dwState & EV_IS_VER2)
			{
				for (int indexOfParam = 0; indexOfParam < event.m_nArgCount; indexOfParam++)
				{
					auto& param = reinterpret_cast<EVENT_INFO2&>(event).m_pEventArgInfo[indexOfParam];
					writer.StartObject();
					writer.Key(L"Name");
					AnsiString(writer, param.m_szName, codepage);
					writer.Key(L"DataType");
					writer.Int(param.m_dtDataType);
					if (param.m_szExplain)
					{
						writer.Key(L"Description");
						AnsiString(writer, param.m_szExplain, codepage);
					}
					writer.EndObject();
				}
			}
			else
			{
				for (int indexOfParam = 0; indexOfParam < event.m_nArgCount; indexOfParam++)
				{
					auto& param = event.m_pEventArgInfo[indexOfParam];
					writer.StartObject();
					writer.Key(L"Name");
					AnsiString(writer, param.m_szName, codepage);
					writer.Key(L"DataType");
					if (param.m_dwState & EAS_IS_BOOL_ARG)
					{
						writer.Int(SDT_BOOL);
					}
					else
					{
						writer.Int(SDT_INT);
					}
					if (param.m_szExplain)
					{
						writer.Key(L"Description");
						AnsiString(writer, param.m_szExplain, codepage);
					}
					writer.EndObject();
				}
			}
			writer.EndArray();

			writer.EndObject();
			pEvent += (event.m_dwState & EV_IS_VER2) ? sizeof(EVENT_INFO2) : sizeof(EVENT_INFO);
		}
		writer.EndArray();

		writer.Key(L"Members");
		writer.StartArray();
		if (type.m_nPropertyCount != 0)
		{
			for (int j = 0; j < type.m_nPropertyCount; j++)
			{
				auto& prop = type.m_pPropertyBegin[j];
				writer.StartObject();
				writer.Key(L"Name");
				AnsiString(writer, prop.m_szName, codepage);
				writer.Key(L"EnglishName");
				AnsiString(writer, prop.m_szEgName, codepage);
				
				writer.Key(L"DataType");
				writer.Int(UnitPropertyTypeToDataType(pLibInfo, prop.m_shtType));

				if (prop.m_wState & UW_IS_HIDED)
				{
					writer.Key(L"Deprecated");
					writer.String(L"Hidden");
				}

				if (prop.m_szExplain)
				{
					writer.Key(L"Description");
					AnsiString(writer, prop.m_szExplain, codepage);
				}
				writer.EndObject();
			}
		}
		else
		{
			for (int j = 0; j < type.m_nElementCount; j++)
			{
				auto& member = type.m_pElementBegin[j];
				writer.StartObject();
				writer.Key(L"Name");
				AnsiString(writer, member.m_szName, codepage);
				writer.Key(L"EnglishName");
				AnsiString(writer, member.m_szEgName, codepage);
				writer.Key(L"DataType");
				writer.Int(member.m_dtType);

				if (member.m_dwState & LES_HIDED)
				{
					writer.Key(L"Deprecated");
					writer.String(L"Hidden");
				}

				if (member.m_szExplain)
				{
					writer.Key(L"Description");
					AnsiString(writer, member.m_szExplain, codepage);
				}
				if (member.m_dwState & LES_HAS_DEFAULT_VALUE || type.m_dwState & LDT_ENUM)
				{
					WriteDefaultKVPair(writer, member.m_dtType, member.m_nDefault, codepage);
				}
				writer.EndObject();
			}
		}
		writer.EndArray();

		writer.Key(L"Methods");
		writer.StartArray();
		for (int j = 0; j < type.m_nCmdCount; j++)
		{
			writer.Int(type.m_pnCmdsIndex[j]);
		}
		writer.EndArray();

		writer.EndObject();
	}
	writer.EndArray();

	writer.Key(L"Categories");
	writer.StartArray();
	auto pCategory = pLibInfo->m_szzCategory;
	for (int i = 0; i < pLibInfo->m_nCategoryCount; i++)
	{
		auto lenOfRawCategoryInfo = strlen(pCategory);
		writer.StartObject();
		if (lenOfRawCategoryInfo >= 4)
		{
			auto imageId = std::stoi(std::string(pCategory, 4));
			if (imageId != 0)
			{
				writer.Key(L"ImageId");
				writer.Int(imageId);
			}
			writer.Key(L"Name");
			AnsiString(writer, &pCategory[4], codepage);
		}
		writer.EndObject();

		pCategory = pCategory + lenOfRawCategoryInfo + 1;
	}
	writer.EndArray();

	writer.Key(L"Cmds");
	writer.StartArray();
	for (int i = 0; i < pLibInfo->m_nCmdCount; i++)
	{
		auto& cmd = pLibInfo->m_pBeginCmdInfo[i];
		writer.StartObject();
		writer.Key(L"Name");
		AnsiString(writer, cmd.m_szName, codepage);
		writer.Key(L"EnglshName");
		AnsiString(writer, cmd.m_szEgName, codepage);
		if (cmd.m_szExplain)
		{
			writer.Key(L"Description");
			AnsiString(writer, cmd.m_szExplain, codepage);
		}
		writer.Key(L"LearningCost");
		switch (cmd.m_shtUserLevel)
		{
		case LVL_SIMPLE:
			writer.String(L"Low");
			break;
		case LVL_SECONDARY:
			writer.String(L"Moderate");
			break;
		case LVL_HIGH:
		default:
			writer.String(L"High");
			break;
		}
		if (cmd.m_wState & CT_IS_HIDED)
		{
			writer.Key(L"Deprecated");
			if (cmd.m_wState & CT_IS_ERROR)
			{
				writer.String(L"Error");
			}
			else
			{
				writer.String(L"Hidden");
			}
		}
		if (cmd.m_wState & CT_DISABLED_IN_RELEASE)
		{
			writer.Key(L"DebugOnly");
			writer.Bool(true);
		}
		if (cmd.m_shtCategory != -1)
		{
			writer.Key(L"CategoryId");
			writer.Int(cmd.m_shtCategory);
		}
		writer.Key(L"ReturnDataType");
		writer.Int(cmd.m_dtRetValType);
		if (cmd.m_wState & CT_RETRUN_ARY_TYPE_DATA)
		{
			writer.Key(L"ReturnArray");
			writer.Bool(true);
		}
		writer.Key(L"Parameters");
		writer.StartArray();
		for (int indexOfParam = 0; indexOfParam < cmd.m_nArgCount; indexOfParam++)
		{
			auto& param = cmd.m_pBeginArgInfo[indexOfParam];
			writer.StartObject();
			writer.Key(L"Name");
			AnsiString(writer, param.m_szName, codepage);
			writer.Key(L"DataType");
			writer.Int(param.m_dtType);
			if (param.m_szExplain)
			{
				writer.Key(L"Description");
				AnsiString(writer, param.m_szExplain, codepage);
			}
			writer.Key(L"ReceiveFlags");
			writer.Int(param.m_dwState & 0x03fc);
			if (param.m_dwState & AS_HAS_DEFAULT_VALUE)
			{
				WriteDefaultKVPair(writer, param.m_dtType, param.m_nDefault, codepage);
			}
			if (param.m_dwState & AS_HAS_DEFAULT_VALUE || param.m_dwState & AS_DEFAULT_VALUE_IS_EMPTY)
			{
				writer.Key(L"Optional");
				writer.Bool(true);
			}
			if (cmd.m_wState & CT_ALLOW_APPEND_NEW_ARG && indexOfParam == cmd.m_nArgCount - 1)
			{
				writer.Key(L"VarArgs");
				writer.Bool(true);
			}
			writer.EndObject();
		}
		writer.EndArray();

		if (cmd.m_wState & CT_IS_OBJ_FREE_CMD)
		{
			writer.Key(L"Kind");
			writer.String(L"Destructor");
		}
		else if (cmd.m_wState & CT_IS_OBJ_COPY_CMD)
		{
			writer.Key(L"Kind");
			writer.String(L"Clone");
		}
		else if (cmd.m_wState & CT_IS_OBJ_CONSTURCT_CMD)
		{
			writer.Key(L"Kind");
			writer.String(L"Constructor");
		}

		writer.EndObject();
	}
	writer.EndArray();

	writer.Key(L"Constants");
	writer.StartArray();
	for (int i = 0; i < pLibInfo->m_nLibConstCount; i++)
	{
		auto& constant = pLibInfo->m_pLibConst[i];
		writer.StartObject();
		writer.Key(L"Name");
		AnsiString(writer, constant.m_szName, codepage);
		writer.Key(L"EnglshName");
		AnsiString(writer, constant.m_szEgName, codepage);
		writer.Key(L"Description");
		AnsiString(writer, constant.m_szExplain, codepage);
		switch (constant.m_shtType)
		{
		case CT_NUM:
			if ((int32_t)constant.m_dbValue == constant.m_dbValue)
			{
				writer.Key(L"DataType");
				writer.Int(SDT_INT);
				writer.Key(L"Value");
				writer.Int((int32_t)constant.m_dbValue);
			}
			else if ((int64_t)constant.m_dbValue == constant.m_dbValue)
			{
				writer.Key(L"DataType");
				writer.Int(SDT_INT64);
				writer.Key(L"Value");
				writer.Int64((int64_t)constant.m_dbValue);
			}
			else
			{
				writer.Key(L"DataType");
				writer.Int(SDT_DOUBLE);
				writer.Key(L"Value");
				writer.Double(constant.m_dbValue);
			}
			break;
		case CT_BOOL:
			writer.Key(L"DataType");
			writer.Int(SDT_BOOL);
			writer.Key(L"Value");
			writer.Bool(constant.m_dbValue != 0);
			break;
		case CT_TEXT:
			writer.Key(L"DataType");
			writer.Int(SDT_TEXT);
			writer.Key(L"Value");
			AnsiString(writer, constant.m_szText, codepage);
			break;
		case CT_NULL:
		default:
			writer.Key(L"Value");
			writer.Null();
			break;
		}
		writer.EndObject();
	}
	writer.EndArray();

	writer.EndObject();

	FreeLibrary(fne);

	auto result = s.GetString();
	if (hFile)
	{
		DWORD numberOfBytesWritten;
		WriteFile(hFile, L"\ufeff", sizeof(wchar_t), &numberOfBytesWritten, nullptr);
		WriteFile(hFile, result, s.GetSize(), &numberOfBytesWritten, nullptr);
		CloseHandle(hFile);
	}
	else
	{
		std::wcout << result;
	}
	return 0;
}

