# PowerShell Script to Generate Valid Tax Code (Codice Fiscale) and IBAN
# Usage: .\generate-valid-codes.ps1

function Generate-TaxCode {
    param(
        [string]$FirstName = "Mario",
        [string]$LastName = "Rossi",
        [char]$Gender = 'M',
        [DateTime]$BirthDate = (Get-Date "1980-01-15"),
        [string]$BirthPlace = "Milano"
    )
    
    # Consonants and vowels maps
    $consonants = "BCDFGHJKLMNPQRSTVWXYZ"
    $vowels = "AEIOU"
    
    # Month codes
    $monthCodes = @{
        1 = 'A'; 2 = 'B'; 3 = 'C'; 4 = 'D'; 5 = 'E'; 6 = 'H';
        7 = 'L'; 8 = 'M'; 9 = 'P'; 10 = 'R'; 11 = 'S'; 12 = 'T'
    }
    
    # Place codes (simplified - using H501 for Milano as example)
    $placeCodes = @{
        "Milano" = "F205"
        "Roma" = "H501"
        "Napoli" = "F839"
        "Torino" = "L219"
        "Palermo" = "G273"
        "Genova" = "D969"
        "Bologna" = "A944"
        "Firenze" = "D612"
        "Bari" = "A662"
        "Catania" = "C351"
    }
    
    function Get-ConsonantsAndVowels($text) {
        $text = $text.ToUpper() -replace '[^A-Z]', ''
        $cons = ""
        $vow = ""
        
        foreach ($char in $text.ToCharArray()) {
            if ($consonants.Contains($char)) {
                $cons += $char
            } elseif ($vowels.Contains($char)) {
                $vow += $char
            }
        }
        
        return @($cons, $vow)
    }
    
    function Get-NameCode($name) {
        $result = Get-ConsonantsAndVowels $name
        $cons = $result[0]
        $vow = $result[1]
        
        if ($cons.Length -ge 3) {
            return $cons.Substring(0, 3)
        } elseif ($cons.Length + $vow.Length -ge 3) {
            return ($cons + $vow).Substring(0, 3)
        } else {
            return ($cons + $vow + "XXX").Substring(0, 3)
        }
    }
    
    function Get-SurnameCode($surname) {
        $result = Get-ConsonantsAndVowels $surname
        $cons = $result[0]
        $vow = $result[1]
        
        # For surname, take first 4 consonants if available, otherwise use rule for names
        if ($cons.Length -ge 4) {
            return $cons.Substring(0, 3)
        } elseif ($cons.Length -ge 3) {
            return $cons.Substring(0, 3)
        } elseif ($cons.Length + $vow.Length -ge 3) {
            return ($cons + $vow).Substring(0, 3)
        } else {
            return ($cons + $vow + "XXX").Substring(0, 3)
        }
    }
    
    # Build tax code parts
    $surnameCode = Get-SurnameCode $LastName
    $nameCode = Get-NameCode $FirstName
    $yearCode = $BirthDate.Year.ToString().Substring(2, 2)
    $monthCode = $monthCodes[$BirthDate.Month]
    
    # Day code (for females add 40)
    $dayCode = $BirthDate.Day
    if ($Gender -eq 'F') {
        $dayCode += 40
    }
    $dayCode = $dayCode.ToString("D2")
    
    # Place code
    $placeCode = if ($placeCodes.ContainsKey($BirthPlace)) { 
        $placeCodes[$BirthPlace] 
    } else { 
        "H501"  # Default to Roma
    }
    
    # Calculate check digit
    $partialCode = $surnameCode + $nameCode + $yearCode + $monthCode + $dayCode + $placeCode
    $checkDigit = Calculate-TaxCodeCheckDigit $partialCode
    
    return $partialCode + $checkDigit
}

function Calculate-TaxCodeCheckDigit($code) {
    $oddChars = @{
        '0' = 1; '1' = 0; '2' = 5; '3' = 7; '4' = 9; '5' = 13; '6' = 15; '7' = 17; '8' = 19; '9' = 21;
        'A' = 1; 'B' = 0; 'C' = 5; 'D' = 7; 'E' = 9; 'F' = 13; 'G' = 15; 'H' = 17; 'I' = 19; 'J' = 21;
        'K' = 2; 'L' = 4; 'M' = 18; 'N' = 20; 'O' = 11; 'P' = 3; 'Q' = 6; 'R' = 8; 'S' = 12; 'T' = 14;
        'U' = 16; 'V' = 10; 'W' = 22; 'X' = 25; 'Y' = 24; 'Z' = 23
    }
    
    $evenChars = @{
        '0' = 0; '1' = 1; '2' = 2; '3' = 3; '4' = 4; '5' = 5; '6' = 6; '7' = 7; '8' = 8; '9' = 9;
        'A' = 0; 'B' = 1; 'C' = 2; 'D' = 3; 'E' = 4; 'F' = 5; 'G' = 6; 'H' = 7; 'I' = 8; 'J' = 9;
        'K' = 10; 'L' = 11; 'M' = 12; 'N' = 13; 'O' = 14; 'P' = 15; 'Q' = 16; 'R' = 17; 'S' = 18; 'T' = 19;
        'U' = 20; 'V' = 21; 'W' = 22; 'X' = 23; 'Y' = 24; 'Z' = 25
    }
    
    $checkChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    
    $sum = 0
    for ($i = 0; $i -lt $code.Length; $i++) {
        $char = $code[$i]
        if ($i % 2 -eq 0) {  # Odd position (1-based)
            $sum += $oddChars[$char.ToString()]
        } else {  # Even position (1-based)
            $sum += $evenChars[$char.ToString()]
        }
    }
    
    $remainder = $sum % 26
    return $checkChars[$remainder]
}

function Generate-IBAN {
    param(
        [string]$CountryCode = "IT",
        [string]$BankCode = "05428",
        [string]$BranchCode = "11101"
    )
    
    # Generate random account number (12 digits for Italian IBAN)
    $accountNumber = ""
    for ($i = 0; $i -lt 12; $i++) {
        $accountNumber += Get-Random -Minimum 0 -Maximum 10
    }
    
    # Calculate check digits
    $bban = $BankCode + $BranchCode + $accountNumber
    $checkDigits = Calculate-IBANCheckDigits $CountryCode $bban
    
    return $CountryCode + $checkDigits + $bban
}

function Calculate-IBANCheckDigits($countryCode, $bban) {
    # Convert letters to numbers (A=10, B=11, ..., Z=35)
    function Convert-LetterToNumber($letter) {
        return [int][char]$letter - [int][char]'A' + 10
    }
    
    # Move first 4 characters to end and replace letters with numbers
    $rearranged = $bban + $countryCode + "00"
    $numeric = ""
    
    foreach ($char in $rearranged.ToCharArray()) {
        if ([char]::IsLetter($char)) {
            $numeric += Convert-LetterToNumber $char
        } else {
            $numeric += $char
        }
    }
    
    # Calculate mod 97
    $remainder = [BigInt]::Parse($numeric) % 97
    $checkDigits = 98 - $remainder
    
    return $checkDigits.ToString("D2")
}

# Main script execution
Write-Host "=== Tax Code and IBAN Generator ===" -ForegroundColor Green
Write-Host ""

# Generate sample data
$samplePersons = @(
    @{ FirstName = "Mario"; LastName = "Rossi"; Gender = 'M'; BirthDate = Get-Date "1980-01-15"; BirthPlace = "Milano" },
    @{ FirstName = "Giulia"; LastName = "Bianchi"; Gender = 'F'; BirthDate = Get-Date "1992-06-23"; BirthPlace = "Roma" },
    @{ FirstName = "Francesco"; LastName = "Verdi"; Gender = 'M'; BirthDate = Get-Date "1975-11-08"; BirthPlace = "Napoli" },
    @{ FirstName = "Alessandra"; LastName = "Ferrari"; Gender = 'F'; BirthDate = Get-Date "1988-03-12"; BirthPlace = "Torino" }
)

foreach ($person in $samplePersons) {
    Write-Host "Person: $($person.FirstName) $($person.LastName)" -ForegroundColor Cyan
    Write-Host "Birth: $($person.BirthDate.ToString('dd/MM/yyyy')) in $($person.BirthPlace)" -ForegroundColor Gray
    
    # Generate Tax Code
    $taxCode = Generate-TaxCode -FirstName $person.FirstName -LastName $person.LastName -Gender $person.Gender -BirthDate $person.BirthDate -BirthPlace $person.BirthPlace
    Write-Host "Tax Code (Codice Fiscale): $taxCode" -ForegroundColor Yellow
    
    # Generate IBAN
    $iban = Generate-IBAN
    Write-Host "IBAN: $iban" -ForegroundColor Yellow
    
    Write-Host ""
}

# Interactive mode
Write-Host "=== Interactive Mode ===" -ForegroundColor Green
$continue = Read-Host "Do you want to generate a custom tax code and IBAN? (y/n)"

if ($continue -eq 'y' -or $continue -eq 'Y') {
    $firstName = Read-Host "Enter first name"
    $lastName = Read-Host "Enter last name"
    $gender = Read-Host "Enter gender (M/F)"
    $birthDateStr = Read-Host "Enter birth date (dd/MM/yyyy)"
    $birthPlace = Read-Host "Enter birth place"
    
    try {
        $birthDate = [DateTime]::ParseExact($birthDateStr, "dd/MM/yyyy", $null)
        
        Write-Host "`nGenerating codes for: $firstName $lastName" -ForegroundColor Cyan
        
        $customTaxCode = Generate-TaxCode -FirstName $firstName -LastName $lastName -Gender $gender[0] -BirthDate $birthDate -BirthPlace $birthPlace
        Write-Host "Tax Code: $customTaxCode" -ForegroundColor Yellow
        
        $customIban = Generate-IBAN
        Write-Host "IBAN: $customIban" -ForegroundColor Yellow
        
    } catch {
        Write-Host "Error: Invalid date format. Please use dd/MM/yyyy" -ForegroundColor Red
    }
}

Write-Host "`n=== Script completed ===" -ForegroundColor Green
