namespace BloodDonation.Infrastructure.Gemini;

public static class MedicalPrompt
{
    public static string Get()
    {
        return """
        You are a medical document verification expert.

        Analyze this medical document image carefully.

        Return ONLY a JSON object:

        {
          "isValidMedicalDocument": true or false,
          "containsBloodRequest": true or false,
          "bloodType": "A+ or B- etc or null",
          "unitsNeeded": number or null,
          "urgencyLevel": "Critical or Urgent or Normal",
          "confidence": number between 0 and 1,
          "rejectionReason": "reason or null"
        }
        """;
    }
}