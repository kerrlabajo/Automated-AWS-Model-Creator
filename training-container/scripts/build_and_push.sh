
ACCOUNT_ID=$1
REGION=$2
REPO_NAME=$3
IMAGE_TAG=$4

# Check if all arguments are provided
if [ -z "$ACCOUNT_ID" ] || [ -z "$REGION" ] || [ -z "$REPO_NAME" ] || [ -z "$IMAGE_TAG" ]; then
    echo "Usage: $0 <account_id> <region> <repo_name> <image_tag>"
    exit 1
fi

# Build ubuntu-cuda and yolov5 images
docker build -f ../docker/ubuntu-cuda/Dockerfile -t ubuntu-cuda ../docker/ubuntu-cuda
docker build -f ../docker/yolov5/Dockerfile -t $REPO_NAME ../docker/yolov5

# Tag image according to user's id and region
docker tag $REPO_NAME $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPO_NAME:$IMAGE_TAG

# Login to AWS ECR and docker credentials
# Locate existing repository or create a new one
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com
aws ecr describe-repositories --repository-names $REPO_NAME > /dev/null 2>&1 || aws ecr create-repository --repository-name $REPO_NAME > /dev/null 2>&1

# Push image to AWS ECR
docker push $ACCOUNT_ID.dkr.ecr.$REGION.amazonaws.com/$REPO_NAME:$IMAGE_TAG

# Delete all Docker images and remove all Docker cache
docker rmi $(docker images -q) -f
docker system prune -a --volumes -f