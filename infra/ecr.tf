resource "aws_ecr_repository" "sign_up_auth" {
    name                 = "sign-up-registry"
    image_tag_mutability = "MUTABLE"
    image_scanning_configuration {
        scan_on_push = true
    }
}