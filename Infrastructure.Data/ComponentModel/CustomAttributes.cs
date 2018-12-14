using System;

namespace Infrastructure.Data.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ClassOwnerAttribute : Attribute
	{
		public string owner;
		public ClassOwnerAttribute(string mOwner)
		{
			this.owner = mOwner;
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	public sealed class IsTestedAttribute : Attribute
	{
		public bool isTested;
		public IsTestedAttribute(bool isTested)
		{
			this.isTested = isTested;
		}

		public string Reminder { get; set; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public sealed class YetkiKontroluYapilmayacak : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class TabloAliasAttribute : Attribute
	{
		public string tabloAlias;
		public TabloAliasAttribute(string tabloAlias)
		{
			this.tabloAlias = tabloAlias;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IsFlaggedEnumField : Attribute
	{
		public bool isFlaggedEnum;
		public IsFlaggedEnumField(bool isFlaggedEnum)
		{
			this.isFlaggedEnum = isFlaggedEnum;
		}
	}

	/// <summary>
	/// Search Criteria'larda ana tabloya ait tanım tablolarını birbirine eşleştirmek için kullanılan Attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RelatedDefinitionTable : Attribute
	{
		public string definitionTableName;
		public RelatedDefinitionTable(string definitionTableName)
		{
			this.definitionTableName = definitionTableName;
		}
	}

	/// <summary>
	/// IB.EBakanlik.Types.Enumerations.Ortak.DataTypeSqlUsage altındaki struct'ı kullanınız.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DataTypeSqlUsageAttribute : Attribute
	{
		public string typeUsage;
		public DataTypeSqlUsageAttribute(string typeUsage)
		{
			this.typeUsage = typeUsage;
		}
	}

	/// <summary>
	/// IB.EBakanlik.Types.CustomTypes.BaseSearchCriteria içinde Between kullanıalcak durumlarda, property olarak XBaslangic ve XBitis verildiği zaman
	/// Bu attiribute sayesinde asıl alanın adı X olarak alınabilceği için between kullanımı daha kolaylaşıcak.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RealFieldName : Attribute
	{
		public string realFieldName;
		public byte dateType;

	    /// <summary>
		/// </summary>
		/// <param name="realFieldName">Between olarak kullanılacak kolon adının kök hali Ör:KayitTarihi</param>
		/// <param name="dateType">Between kullanılırken,bu kolon Başlangıçmı Bitişmi olduğunu belli edecek.Başlangıç = 1 , Bitiş = 2</param>
		public RealFieldName(string realFieldName, byte dateType)
		{
			this.realFieldName = realFieldName;
			this.dateType = dateType;
		}
	}
}
