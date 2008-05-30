//Chandler Wall (cswall@purdue.edu)
//SID: 00162-59681
//Div/Sec: 01/01
//Date: 03/20/08
//HW7: sorted list using a ternary tree

#include<stdio.h>

//structure for each node of the ternary tree
struct node
{
  int data;
  struct node *left;
  struct node *right;
  struct node *middle;
};

typedef struct node node;

node* storeInt(node*, int);
node* searchNode(node*, int);
void printNode(node*);

int main(void){
  int n, i, data;
  node *root, *new;
  root = NULL;

  //prompt the user to input an integer until -1 is entered
  printf("Enter an integer to store: ");
  scanf("%d", &data);
  while(data != -1){
    //store the integer in the tree
    root = storeInt(root, data);
    //print the current tree (sorted list format)
    printNode(root);
    printf("\nEnter an integer to store: ");
    scanf("%d", &data);
  }
  printNode(root);
  
  return(0);
}
node* storeInt(node *root, int data)
{
  node *node_new, *node_selected;
  char found;
  
  //allocate space for a new tree node
  node_new = (node*)malloc(sizeof(node));
  
  if(node_new == NULL)
  {
    printf("ERROR: New tree node was not allocated.\n");
    exit(0);
  }
  
  node_new->data = data;
  node_new->left = NULL;
  node_new->right = NULL;
  node_new->middle = NULL;
  
  //if there are no current nodes, set the root equal to the new node
  if(root == NULL){
    root = node_new;
  }else{
    //determine the appropiate position of the data (left, middle, right)
    //place the data in the first available null node
    node_selected = root;
    found = 0;
    while(found == 0){
      if(data < node_selected->data){
        if(node_selected->left == NULL){
          node_selected->left = node_new;
          found = 1;
        }else{
          node_selected = node_selected->left;
        }
      }else if(data == node_selected->data){
        if(node_selected->middle == NULL){
          node_selected->middle = node_new;
          found = 1;
        }else{
          node_selected = node_selected->middle;
        }
      }else if(data > node_selected->data){
        if(node_selected->right == NULL){
          node_selected->right = node_new;
          found = 1;
        }else{
          node_selected = node_selected->right;
        }
      }
    }
  }
  
  return(root);
}
void printNode(node *curr)
{
  //recursive function to print the data in the tree (left to middle to right)
  if(curr != NULL){
    printNode(curr->left);
    printNode(curr->middle);
    printf("%d ", curr->data);
    printNode(curr->right);
  }
}
