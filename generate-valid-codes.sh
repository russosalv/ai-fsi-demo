#!/bin/bash

# Bash Script to Generate Valid Tax Code (Codice Fiscale) and IBAN
# Usage: ./generate-valid-codes.sh

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Function to generate tax code
generate_tax_code() {
    local first_name="$1"
    local last_name="$2"
    local gender="$3"
    local birth_date="$4"
    local birth_place="$5"
    
    # Convert to uppercase and remove non-alphabetic characters
    first_name=$(echo "$first_name" | tr '[:lower:]' '[:upper:]' | sed 's/[^A-Z]//g')
    last_name=$(echo "$last_name" | tr '[:lower:]' '[:upper:]' | sed 's/[^A-Z]//g')
    
    # Extract consonants and vowels
    get_consonants_vowels() {
        local text="$1"
        local consonants=""
        local vowels=""
        
        for ((i=0; i<${#text}; i++)); do
            char="${text:$i:1}"
            case "$char" in
                [BCDFGHJKLMNPQRSTVWXYZ]) consonants="$consonants$char" ;;
                [AEIOU]) vowels="$vowels$char" ;;
            esac
        done
        
        echo "$consonants|$vowels"
    }
    
    # Get name code (3 chars)
    get_name_code() {
        local name="$1"
        local result=$(get_consonants_vowels "$name")
        local cons="${result%|*}"
        local vow="${result#*|}"
        
        # For first names with 4+ consonants, take 1st, 3rd, 4th
        if [ ${#cons} -ge 4 ]; then
            echo "${cons:0:1}${cons:2:1}${cons:3:1}"
        elif [ ${#cons} -ge 3 ]; then
            echo "${cons:0:3}"
        elif [ $((${#cons} + ${#vow})) -ge 3 ]; then
            local combined="$cons$vow"
            echo "${combined:0:3}"
        else
            local combined="${cons}${vow}XXX"
            echo "${combined:0:3}"
        fi
    }
    
    # Get surname code (3 chars)
    get_surname_code() {
        local surname="$1"
        local result=$(get_consonants_vowels "$surname")
        local cons="${result%|*}"
        local vow="${result#*|}"
        
        if [ ${#cons} -ge 3 ]; then
            echo "${cons:0:3}"
        elif [ $((${#cons} + ${#vow})) -ge 3 ]; then
            local combined="$cons$vow"
            echo "${combined:0:3}"
        else
            local combined="${cons}${vow}XXX"
            echo "${combined:0:3}"
        fi
    }
    
    # Parse birth date (format: DD/MM/YYYY)
    local day="${birth_date:0:2}"
    local month="${birth_date:3:2}"
    local year="${birth_date:6:4}"
    
    # Build tax code parts
    local surname_code=$(get_surname_code "$last_name")
    local name_code=$(get_name_code "$first_name")
    local year_code="${year:2:2}"
    
    # Month code
    case "$month" in
        "01") month_code="A" ;;
        "02") month_code="B" ;;
        "03") month_code="C" ;;
        "04") month_code="D" ;;
        "05") month_code="E" ;;
        "06") month_code="H" ;;
        "07") month_code="L" ;;
        "08") month_code="M" ;;
        "09") month_code="P" ;;
        "10") month_code="R" ;;
        "11") month_code="S" ;;
        "12") month_code="T" ;;
    esac
    
    # Day code (add 40 for females)
    local day_code="$day"
    if [ "$gender" = "F" ]; then
        day_code=$((day + 40))
    fi
    day_code=$(printf "%02d" "$day_code")
    
    # Place code (simplified mapping)
    case "$birth_place" in
        "Milano"|"MILANO") place_code="F205" ;;
        "Roma"|"ROMA") place_code="H501" ;;
        "Napoli"|"NAPOLI") place_code="F839" ;;
        "Torino"|"TORINO") place_code="L219" ;;
        "Palermo"|"PALERMO") place_code="G273" ;;
        "Genova"|"GENOVA") place_code="D969" ;;
        "Bologna"|"BOLOGNA") place_code="A944" ;;
        "Firenze"|"FIRENZE") place_code="D612" ;;
        "Bari"|"BARI") place_code="A662" ;;
        "Catania"|"CATANIA") place_code="C351" ;;
        *) place_code="H501" ;;  # Default to Roma
    esac
    
    # Calculate check digit
    local partial_code="$surname_code$name_code$year_code$month_code$day_code$place_code"
    local check_digit=$(calculate_tax_code_check_digit "$partial_code")
    
    echo "$partial_code$check_digit"
}

# Function to calculate tax code check digit
calculate_tax_code_check_digit() {
    local code="$1"
    local sum=0
    
    # Lookup tables for odd and even positions
    declare -A odd_values=(
        ["0"]=1 ["1"]=0 ["2"]=5 ["3"]=7 ["4"]=9 ["5"]=13 ["6"]=15 ["7"]=17 ["8"]=19 ["9"]=21
        ["A"]=1 ["B"]=0 ["C"]=5 ["D"]=7 ["E"]=9 ["F"]=13 ["G"]=15 ["H"]=17 ["I"]=19 ["J"]=21
        ["K"]=2 ["L"]=4 ["M"]=18 ["N"]=20 ["O"]=11 ["P"]=3 ["Q"]=6 ["R"]=8 ["S"]=12 ["T"]=14
        ["U"]=16 ["V"]=10 ["W"]=22 ["X"]=25 ["Y"]=24 ["Z"]=23
    )
    
    declare -A even_values=(
        ["0"]=0 ["1"]=1 ["2"]=2 ["3"]=3 ["4"]=4 ["5"]=5 ["6"]=6 ["7"]=7 ["8"]=8 ["9"]=9
        ["A"]=0 ["B"]=1 ["C"]=2 ["D"]=3 ["E"]=4 ["F"]=5 ["G"]=6 ["H"]=7 ["I"]=8 ["J"]=9
        ["K"]=10 ["L"]=11 ["M"]=12 ["N"]=13 ["O"]=14 ["P"]=15 ["Q"]=16 ["R"]=17 ["S"]=18 ["T"]=19
        ["U"]=20 ["V"]=21 ["W"]=22 ["X"]=23 ["Y"]=24 ["Z"]=25
    )
    
    local check_chars="ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    
    for ((i=0; i<${#code}; i++)); do
        local char="${code:$i:1}"
        if [ $((i % 2)) -eq 0 ]; then  # Odd position (1-based)
            sum=$((sum + ${odd_values[$char]}))
        else  # Even position (1-based)
            sum=$((sum + ${even_values[$char]}))
        fi
    done
    
    local remainder=$((sum % 26))
    echo "${check_chars:$remainder:1}"
}

# Function to generate IBAN
generate_iban() {
    local country_code="${1:-IT}"
    local abi_code="${2:-05428}"
    local cab_code="${3:-11101}"
    
    # Generate random account number (12 digits for Italian IBAN)
    local account_number=""
    for ((i=0; i<12; i++)); do
        account_number="$account_number$((RANDOM % 10))"
    done
    
    # Calculate CIN (Control Internal Number) for Italian IBAN
    local cin_code=$(calculate_italian_cin "$abi_code$cab_code$account_number")
    
    # Build BBAN (Basic Bank Account Number): CIN + ABI + CAB + Account
    local bban="$cin_code$abi_code$cab_code$account_number"
    
    # Calculate IBAN check digits
    local check_digits=$(calculate_iban_check_digits "$country_code" "$bban")
    
    echo "$country_code$check_digits$bban"
}

# Function to calculate Italian CIN
calculate_italian_cin() {
    local account_code="$1"
    local sum=0
    
    # Lookup tables for odd and even positions (for CIN calculation)
    declare -a odd_weights=(1 0 5 7 9 13 15 17 19 21 2 4 18 20 11 3 6 8 12 14 16 10 22 25 24 23)
    declare -a even_weights=(0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25)
    local cin_chars="ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    
    for ((i=0; i<${#account_code}; i++)); do
        local char="${account_code:$i:1}"
        local value=0
        
        if [[ "$char" =~ [0-9] ]]; then
            value=$((char))
        elif [[ "$char" =~ [A-Z] ]]; then
            value=$(($(printf "%d" "'$char") - $(printf "%d" "'A") + 10))
        fi
        
        if [ $((i % 2)) -eq 0 ]; then  # Odd position (1-based)
            sum=$((sum + ${odd_weights[$value]}))
        else  # Even position (1-based)
            sum=$((sum + ${even_weights[$value]}))
        fi
    done
    
    local remainder=$((sum % 26))
    echo "${cin_chars:$remainder:1}"
}

# Function to calculate IBAN check digits
calculate_iban_check_digits() {
    local country_code="$1"
    local bban="$2"
    
    # Convert letters to numbers (A=10, B=11, ..., Z=35)
    convert_letter_to_number() {
        local letter="$1"
        printf "%d" "'$letter"
        echo $(($(printf "%d" "'$letter") - $(printf "%d" "'A") + 10))
    }
    
    # Move first 4 characters to end and replace letters with numbers
    local rearranged="${bban}${country_code}00"
    local numeric=""
    
    for ((i=0; i<${#rearranged}; i++)); do
        local char="${rearranged:$i:1}"
        if [[ "$char" =~ [A-Z] ]]; then
            local num=$(($(printf "%d" "'$char") - $(printf "%d" "'A") + 10))
            numeric="$numeric$num"
        else
            numeric="$numeric$char"
        fi
    done
    
    # Calculate mod 97 using bc for large numbers
    local remainder=$(echo "$numeric % 97" | bc)
    local check_digits=$((98 - remainder))
    
    printf "%02d" "$check_digits"
}

# Check if bc is installed
check_dependencies() {
    if ! command -v bc &> /dev/null; then
        echo -e "${RED}Error: 'bc' calculator is required but not installed.${NC}"
        echo "Please install it using:"
        echo "  Ubuntu/Debian: sudo apt-get install bc"
        echo "  CentOS/RHEL: sudo yum install bc"
        echo "  macOS: brew install bc"
        exit 1
    fi
}

# Main script execution
main() {
    check_dependencies
    
    echo -e "${GREEN}=== Tax Code and IBAN Generator ===${NC}"
    echo ""
    
    # Sample persons data
    declare -a sample_persons=(
        "Mario|Rossi|M|15/01/1980|Milano"
        "Giulia|Bianchi|F|23/06/1992|Roma"
        "Francesco|Verdi|M|08/11/1975|Napoli"
        "Alessandra|Ferrari|F|12/03/1988|Torino"
    )
    
    # Generate codes for sample persons
    for person_data in "${sample_persons[@]}"; do
        IFS='|' read -r first_name last_name gender birth_date birth_place <<< "$person_data"
        
        echo -e "${CYAN}Person: $first_name $last_name${NC}"
        echo -e "${GRAY}Birth: $birth_date in $birth_place${NC}"
        
        # Generate Tax Code
        tax_code=$(generate_tax_code "$first_name" "$last_name" "$gender" "$birth_date" "$birth_place")
        echo -e "${YELLOW}Tax Code (Codice Fiscale): $tax_code${NC}"
        
        # Generate IBAN
        iban=$(generate_iban)
        echo -e "${YELLOW}IBAN: $iban${NC}"
        
        echo ""
    done
    
    # Interactive mode
    echo -e "${GREEN}=== Interactive Mode ===${NC}"
    read -p "Do you want to generate a custom tax code and IBAN? (y/n): " continue_choice
    
    if [[ "$continue_choice" =~ ^[Yy]$ ]]; then
        read -p "Enter first name: " first_name
        read -p "Enter last name: " last_name
        read -p "Enter gender (M/F): " gender
        read -p "Enter birth date (DD/MM/YYYY): " birth_date
        read -p "Enter birth place: " birth_place
        
        # Validate date format
        if [[ ! "$birth_date" =~ ^[0-9]{2}/[0-9]{2}/[0-9]{4}$ ]]; then
            echo -e "${RED}Error: Invalid date format. Please use DD/MM/YYYY${NC}"
            exit 1
        fi
        
        echo ""
        echo -e "${CYAN}Generating codes for: $first_name $last_name${NC}"
        
        custom_tax_code=$(generate_tax_code "$first_name" "$last_name" "$gender" "$birth_date" "$birth_place")
        echo -e "${YELLOW}Tax Code: $custom_tax_code${NC}"
        
        custom_iban=$(generate_iban)
        echo -e "${YELLOW}IBAN: $custom_iban${NC}"
    fi
    
    echo ""
    echo -e "${GREEN}=== Script completed ===${NC}"
}

# Run main function
main "$@"
