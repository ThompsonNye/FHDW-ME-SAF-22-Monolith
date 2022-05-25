Feature: CarManagement
Manage car (CRUD operations)

    Scenario: 01 Create cars
        Given the following cars
          | Id                                   | Name    |
          | 09b9622a-4182-4ec1-ad90-780593000af1 | Ferrari |
          | 4fd3a81d-d35c-4f57-b591-737fb216b682 | Porsche |
        When I create those cars
        Then those cars get created successfully

    Scenario: 02 Get my cars
        Given the following cars in the system
          | Id                                   | Name    |
          | 09b9622a-4182-4ec1-ad90-780593000af3 | Ferrari |
          | 4fd3a81d-d35c-4f57-b591-737fb216b684 | Porsche |
        When I query the system for my cars
        Then my cars get returned successfully

    Scenario: 03 Update cars
        Given the following cars in the system
          | Id                                   | Name    |
          | 09b9622a-4182-4ec1-ad90-780593000af5 | Ferrari |
          | 4fd3a81d-d35c-4f57-b591-737fb216b686 | Porsche |
        And the following values to update
          | Id                                   | Name |
          | 09b9622a-4182-4ec1-ad90-780593000af5 | BMW  |
          | 4fd3a81d-d35c-4f57-b591-737fb216b686 | Audi |
        When I update those cars with those values
        Then the cars get updated successfully

    Scenario: 04 Delete cars
        Given the following cars in the system
          | Id                                   | Name    |
          | 09b9622a-4182-4ec1-ad90-780593000af7 | Ferrari |
          | 4fd3a81d-d35c-4f57-b591-737fb216b688 | Porsche |
        When I delete those cars
        Then the cars get deleted successfully