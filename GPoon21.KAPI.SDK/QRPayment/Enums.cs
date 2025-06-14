﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace GPoon21.KAPI.SDK.QRPayment;

[JsonConverter(typeof(StatusCodeJsonConverter))]
public enum StatusCode {
    /// <summary>Success</summary>
    Success = 0,

    /// <summary>Error</summary>
    Error = 10
}

public class StatusCodeJsonConverter : JsonConverter<StatusCode> {
    public override StatusCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return StatusCodeExtensions.ParseStatusCode(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, StatusCode value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToCode());
    }
}

public static class StatusCodeExtensions {
    public static string ToCode(this StatusCode status) => status switch {
        StatusCode.Success => "00",
        StatusCode.Error   => "10",
        _                  => throw new ArgumentOutOfRangeException(nameof(status))
    };

    public static StatusCode ParseStatusCode(string code) => code switch {
        "00" => StatusCode.Success,
        "10" => StatusCode.Error,
        _    => throw new ArgumentException($"Invalid status code: {code}", nameof(code))
    };
}

[JsonConverter(typeof(QRTypeJsonConverter))]
public enum QRType {
    /// <summary>Thai QR - Text type</summary>
    ThaiQR = 3,

    /// <summary>Credit Card - Text type</summary>
    CreditCard = 4
}

public class QRTypeJsonConverter : JsonConverter<QRType> {
    public override QRType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return QRTypeExtensions.ParseQRType(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, QRType value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToCode());
    }
}

public static class QRTypeExtensions {
    public static string ToCode(this QRType type) => type switch {
        QRType.ThaiQR     => "3",
        QRType.CreditCard => "4",
        _                 => throw new ArgumentOutOfRangeException(nameof(type))
    };

    public static QRType ParseQRType(string code) => code switch {
        "3" => QRType.ThaiQR,
        "4" => QRType.CreditCard,
        _   => throw new ArgumentException($"Invalid QR type: {code}", nameof(code))
    };
}

[JsonConverter(typeof(ReturnedQRTypeJsonConverter))]
public enum ReturnedQRType {
    /// <summary>Thai QR - Text type</summary>
    ThaiQR = 1,

    /// <summary>Credit Card - Text type</summary>
    CreditCard = 2
}

// Converter for single ReturnedQRType
public class ReturnedQRTypeJsonConverter : JsonConverter<ReturnedQRType> {
    public override ReturnedQRType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return ReturnedQRTypeExtensions.ParseReturnedQRType(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, ReturnedQRType value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToCode());
    }
}

// If you need a separate array converter, create it with a different name
public class ReturnedQRTypeArrayJsonConverter : JsonConverter<ReturnedQRType[]> {
    public override ReturnedQRType[] Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) {
        if (reader.TokenType != JsonTokenType.StartArray) {
            throw new JsonException("Expected start of array");
        }
        List<ReturnedQRType> results = new();
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndArray) {
                break;
            }
            if (reader.TokenType == JsonTokenType.String) {
                results.Add(ReturnedQRTypeExtensions.ParseReturnedQRType(reader.GetString()!));
            }
        }
        return results.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, ReturnedQRType[] value, JsonSerializerOptions options) {
        writer.WriteStartArray();
        foreach (ReturnedQRType val in value) {
            writer.WriteStringValue(val.ToCode());
        }
        writer.WriteEndArray();
    }
}
public static class ReturnedQRTypeExtensions {
    public static string ToCode(this ReturnedQRType type) => type switch {
        ReturnedQRType.ThaiQR     => "PP",
        ReturnedQRType.CreditCard => "CC",
        _                         => throw new ArgumentOutOfRangeException(nameof(type))
    };

    public static ReturnedQRType ParseReturnedQRType(string code) => code?.ToUpperInvariant() switch {
        "PP" => ReturnedQRType.ThaiQR,
        "CC" => ReturnedQRType.CreditCard,
        _    => throw new ArgumentException($"Invalid returned QR type: {code}", nameof(code))
    };
}

[JsonConverter(typeof(TransactionStatusJsonConverter))]
public enum TransactionStatus {
    /// <summary>Transaction has been paid.</summary>
    Paid,

    /// <summary>QR is canceled and cannot be used. For QR Credit Card: Only log as CANCELLED, but QR is still active.</summary>
    Cancelled,

    /// <summary>QR is expired and cannot be used.</summary>
    Expired,

    /// <summary>QR is requested but not yet paid or canceled.</summary>
    Requested,

    /// <summary>Transaction is voided after it is paid.</summary>
    Voided
}

public class TransactionStatusJsonConverter : JsonConverter<TransactionStatus> {
    public override TransactionStatus Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) {
        return TransactionStatusExtensions.ParseTransactionStatus(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, TransactionStatus value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToCode());
    }
}

public static class TransactionStatusExtensions {
    public static string ToCode(this TransactionStatus status) => status switch {
        TransactionStatus.Paid      => "PAID",
        TransactionStatus.Cancelled => "CANCELLED",
        TransactionStatus.Expired   => "EXPIRED",
        TransactionStatus.Requested => "REQUESTED",
        TransactionStatus.Voided    => "VOIDED",
        _                           => throw new ArgumentOutOfRangeException(nameof(status))
    };

    public static TransactionStatus ParseTransactionStatus(string code) => code?.ToUpperInvariant() switch {
        "PAID"      => TransactionStatus.Paid,
        "CANCELLED" => TransactionStatus.Cancelled,
        "EXPIRED"   => TransactionStatus.Expired,
        "REQUESTED" => TransactionStatus.Requested,
        "VOIDED"    => TransactionStatus.Voided,
        _           => throw new ArgumentException($"Invalid transaction status: {code}", nameof(code))
    };


}