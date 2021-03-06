﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsFormsApp1
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="portfolioDB")]
	public partial class LinqToDBDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Определения метода расширяемости
    partial void OnCreated();
    partial void Insertcovariations(covariations instance);
    partial void Updatecovariations(covariations instance);
    partial void Deletecovariations(covariations instance);
    partial void Insertrates(rates instance);
    partial void Updaterates(rates instance);
    partial void Deleterates(rates instance);
    #endregion
		
		public LinqToDBDataContext() : 
				base(global::WindowsFormsApp1.Properties.Settings.Default.portfolioDBConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToDBDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToDBDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToDBDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LinqToDBDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<covariations> covariations
		{
			get
			{
				return this.GetTable<covariations>();
			}
		}
		
		public System.Data.Linq.Table<rates> rates
		{
			get
			{
				return this.GetTable<rates>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.covariations")]
	public partial class covariations : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _ticker_A;
		
		private string _ticker_B;
		
		private System.Nullable<double> _value;
		
		private string _period;
		
		private System.Nullable<System.DateTime> _date;
		
		private System.Nullable<System.TimeSpan> _time;
		
		private System.Nullable<short> _period_analize;
		
    #region Определения метода расширяемости
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void Onticker_AChanging(string value);
    partial void Onticker_AChanged();
    partial void Onticker_BChanging(string value);
    partial void Onticker_BChanged();
    partial void OnvalueChanging(System.Nullable<double> value);
    partial void OnvalueChanged();
    partial void OnperiodChanging(string value);
    partial void OnperiodChanged();
    partial void OndateChanging(System.Nullable<System.DateTime> value);
    partial void OndateChanged();
    partial void OntimeChanging(System.Nullable<System.TimeSpan> value);
    partial void OntimeChanged();
    partial void Onperiod_analizeChanging(System.Nullable<short> value);
    partial void Onperiod_analizeChanged();
    #endregion
		
		public covariations()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ticker_A", DbType="VarChar(128)")]
		public string ticker_A
		{
			get
			{
				return this._ticker_A;
			}
			set
			{
				if ((this._ticker_A != value))
				{
					this.Onticker_AChanging(value);
					this.SendPropertyChanging();
					this._ticker_A = value;
					this.SendPropertyChanged("ticker_A");
					this.Onticker_AChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ticker_B", DbType="VarChar(128)")]
		public string ticker_B
		{
			get
			{
				return this._ticker_B;
			}
			set
			{
				if ((this._ticker_B != value))
				{
					this.Onticker_BChanging(value);
					this.SendPropertyChanging();
					this._ticker_B = value;
					this.SendPropertyChanged("ticker_B");
					this.Onticker_BChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_value", DbType="Float")]
		public System.Nullable<double> value
		{
			get
			{
				return this._value;
			}
			set
			{
				if ((this._value != value))
				{
					this.OnvalueChanging(value);
					this.SendPropertyChanging();
					this._value = value;
					this.SendPropertyChanged("value");
					this.OnvalueChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_period", DbType="VarChar(16)")]
		public string period
		{
			get
			{
				return this._period;
			}
			set
			{
				if ((this._period != value))
				{
					this.OnperiodChanging(value);
					this.SendPropertyChanging();
					this._period = value;
					this.SendPropertyChanged("period");
					this.OnperiodChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_date", DbType="Date")]
		public System.Nullable<System.DateTime> date
		{
			get
			{
				return this._date;
			}
			set
			{
				if ((this._date != value))
				{
					this.OndateChanging(value);
					this.SendPropertyChanging();
					this._date = value;
					this.SendPropertyChanged("date");
					this.OndateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_time", DbType="Time")]
		public System.Nullable<System.TimeSpan> time
		{
			get
			{
				return this._time;
			}
			set
			{
				if ((this._time != value))
				{
					this.OntimeChanging(value);
					this.SendPropertyChanging();
					this._time = value;
					this.SendPropertyChanged("time");
					this.OntimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_period_analize", DbType="SmallInt")]
		public System.Nullable<short> period_analize
		{
			get
			{
				return this._period_analize;
			}
			set
			{
				if ((this._period_analize != value))
				{
					this.Onperiod_analizeChanging(value);
					this.SendPropertyChanging();
					this._period_analize = value;
					this.SendPropertyChanged("period_analize");
					this.Onperiod_analizeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.rates")]
	public partial class rates : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _ticker;
		
		private string _period;
		
		private System.Nullable<System.DateTime> _date;
		
		private System.Nullable<System.TimeSpan> _time;
		
		private System.Nullable<double> _open;
		
		private System.Nullable<double> _high;
		
		private System.Nullable<double> _low;
		
		private System.Nullable<double> _close;
		
		private System.Nullable<int> _volume;
		
		private System.Nullable<double> @__profitable;
		
		private System.Nullable<double> @__profitable_by_period;
		
		private System.Nullable<double> @__risk_by_period;
		
		private System.Nullable<double> @__x;
		
    #region Определения метода расширяемости
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OntickerChanging(string value);
    partial void OntickerChanged();
    partial void OnperiodChanging(string value);
    partial void OnperiodChanged();
    partial void OndateChanging(System.Nullable<System.DateTime> value);
    partial void OndateChanged();
    partial void OntimeChanging(System.Nullable<System.TimeSpan> value);
    partial void OntimeChanged();
    partial void OnopenChanging(System.Nullable<double> value);
    partial void OnopenChanged();
    partial void OnhighChanging(System.Nullable<double> value);
    partial void OnhighChanged();
    partial void OnlowChanging(System.Nullable<double> value);
    partial void OnlowChanged();
    partial void OncloseChanging(System.Nullable<double> value);
    partial void OncloseChanged();
    partial void OnvolumeChanging(System.Nullable<int> value);
    partial void OnvolumeChanged();
    partial void On_profitableChanging(System.Nullable<double> value);
    partial void On_profitableChanged();
    partial void On_profitable_by_periodChanging(System.Nullable<double> value);
    partial void On_profitable_by_periodChanged();
    partial void On_risk_by_periodChanging(System.Nullable<double> value);
    partial void On_risk_by_periodChanged();
    partial void On_xChanging(System.Nullable<double> value);
    partial void On_xChanged();
    #endregion
		
		public rates()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ticker", DbType="VarChar(128)")]
		public string ticker
		{
			get
			{
				return this._ticker;
			}
			set
			{
				if ((this._ticker != value))
				{
					this.OntickerChanging(value);
					this.SendPropertyChanging();
					this._ticker = value;
					this.SendPropertyChanged("ticker");
					this.OntickerChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_period", DbType="VarChar(16)")]
		public string period
		{
			get
			{
				return this._period;
			}
			set
			{
				if ((this._period != value))
				{
					this.OnperiodChanging(value);
					this.SendPropertyChanging();
					this._period = value;
					this.SendPropertyChanged("period");
					this.OnperiodChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_date", DbType="Date")]
		public System.Nullable<System.DateTime> date
		{
			get
			{
				return this._date;
			}
			set
			{
				if ((this._date != value))
				{
					this.OndateChanging(value);
					this.SendPropertyChanging();
					this._date = value;
					this.SendPropertyChanged("date");
					this.OndateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_time", DbType="Time")]
		public System.Nullable<System.TimeSpan> time
		{
			get
			{
				return this._time;
			}
			set
			{
				if ((this._time != value))
				{
					this.OntimeChanging(value);
					this.SendPropertyChanging();
					this._time = value;
					this.SendPropertyChanged("time");
					this.OntimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[open]", Storage="_open", DbType="Float")]
		public System.Nullable<double> open
		{
			get
			{
				return this._open;
			}
			set
			{
				if ((this._open != value))
				{
					this.OnopenChanging(value);
					this.SendPropertyChanging();
					this._open = value;
					this.SendPropertyChanged("open");
					this.OnopenChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_high", DbType="Float")]
		public System.Nullable<double> high
		{
			get
			{
				return this._high;
			}
			set
			{
				if ((this._high != value))
				{
					this.OnhighChanging(value);
					this.SendPropertyChanging();
					this._high = value;
					this.SendPropertyChanged("high");
					this.OnhighChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_low", DbType="Float")]
		public System.Nullable<double> low
		{
			get
			{
				return this._low;
			}
			set
			{
				if ((this._low != value))
				{
					this.OnlowChanging(value);
					this.SendPropertyChanging();
					this._low = value;
					this.SendPropertyChanged("low");
					this.OnlowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[close]", Storage="_close", DbType="Float")]
		public System.Nullable<double> close
		{
			get
			{
				return this._close;
			}
			set
			{
				if ((this._close != value))
				{
					this.OncloseChanging(value);
					this.SendPropertyChanging();
					this._close = value;
					this.SendPropertyChanged("close");
					this.OncloseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_volume", DbType="Int")]
		public System.Nullable<int> volume
		{
			get
			{
				return this._volume;
			}
			set
			{
				if ((this._volume != value))
				{
					this.OnvolumeChanging(value);
					this.SendPropertyChanging();
					this._volume = value;
					this.SendPropertyChanged("volume");
					this.OnvolumeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[_profitable]", Storage="__profitable", DbType="Float")]
		public System.Nullable<double> _profitable
		{
			get
			{
				return this.@__profitable;
			}
			set
			{
				if ((this.@__profitable != value))
				{
					this.On_profitableChanging(value);
					this.SendPropertyChanging();
					this.@__profitable = value;
					this.SendPropertyChanged("_profitable");
					this.On_profitableChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[_profitable_by_period]", Storage="__profitable_by_period", DbType="Float")]
		public System.Nullable<double> _profitable_by_period
		{
			get
			{
				return this.@__profitable_by_period;
			}
			set
			{
				if ((this.@__profitable_by_period != value))
				{
					this.On_profitable_by_periodChanging(value);
					this.SendPropertyChanging();
					this.@__profitable_by_period = value;
					this.SendPropertyChanged("_profitable_by_period");
					this.On_profitable_by_periodChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[_risk_by_period]", Storage="__risk_by_period", DbType="Float")]
		public System.Nullable<double> _risk_by_period
		{
			get
			{
				return this.@__risk_by_period;
			}
			set
			{
				if ((this.@__risk_by_period != value))
				{
					this.On_risk_by_periodChanging(value);
					this.SendPropertyChanging();
					this.@__risk_by_period = value;
					this.SendPropertyChanged("_risk_by_period");
					this.On_risk_by_periodChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[_x]", Storage="__x", DbType="Float")]
		public System.Nullable<double> _x
		{
			get
			{
				return this.@__x;
			}
			set
			{
				if ((this.@__x != value))
				{
					this.On_xChanging(value);
					this.SendPropertyChanging();
					this.@__x = value;
					this.SendPropertyChanged("_x");
					this.On_xChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
