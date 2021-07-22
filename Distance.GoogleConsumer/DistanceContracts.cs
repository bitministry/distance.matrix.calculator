﻿using System.Runtime.Serialization;

namespace GoogleMaps.JSON
{
    //------------------------------------------------------------------------------
    // <auto-generated>
    //     This code was generated by a tool.
    //     Runtime Version:4.0.30319.19408
    //
    //     Changes to this file may cause incorrect behavior and will be lost if
    //     the code is regenerated.
    // </auto-generated>
    //------------------------------------------------------------------------------



    // Type created for JSON at <<root>>
    [DataContract()]
    public partial class GoogleResponse
    {

        [DataMember(Name = "destination_addresses", EmitDefaultValue = false)]
        public string[] destination_addresses;

        [DataMember(Name = "origin_addresses", EmitDefaultValue = false)]
        public string[] origin_addresses;

        [DataMember(Name = "rows", EmitDefaultValue = false)]
        public Rows[] rows;

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string status;
    }

    // Type created for JSON at <<root>> --> rows
    [DataContract(Name="rows")]
    public partial class Rows
    {

        [DataMember(Name = "elements", EmitDefaultValue = false)]
        public Elements[] elements;
    }

    // Type created for JSON at <<root>> --> elements
    [DataContract(Name = "elements")]
    public partial class Elements
    {

        [DataMember(Name = "distance", EmitDefaultValue = false)]
        public Distance distance;

        [DataMember(Name = "duration", EmitDefaultValue = false)]
        public Duration duration;

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string status;
    }

    // Type created for JSON at <<root>> --> distance
    [DataContract(Name = "distance")]
    public partial class Distance
    {

        [DataMember(Name = "text", EmitDefaultValue = false)]
        public string text;

        [DataMember(Name = "value", EmitDefaultValue = false)]
        public int value;
    }

    // Type created for JSON at <<root>> --> duration
    [DataContract(Name = "duration")]
    public partial class Duration
    {

        [DataMember(Name = "text", EmitDefaultValue = false)]
        public string text;

        [DataMember(Name = "value", EmitDefaultValue = false)]
        public int value;
    }
}