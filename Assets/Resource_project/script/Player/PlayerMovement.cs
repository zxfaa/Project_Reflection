using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    const float k_GroundedRadius = .2f;
    private bool m_FacingRight = true;

    [SerializeField] private float runSpeedX = 3f;
    [SerializeField] private float runSpeedY = 1f;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    public bool isLock = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isLock)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = transform.position.z;
                targetPosition = new Vector2(mousePosition.x, mousePosition.y);
                isMoving = true;
                moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            animator.SetBool("isMoving", true);
            Vector2 currentPosition = transform.position;
            float newX = Mathf.MoveTowards(currentPosition.x, targetPosition.x, runSpeedX * Time.deltaTime);
            float newY = Mathf.MoveTowards(currentPosition.y, targetPosition.y, runSpeedY * Time.deltaTime);
            currentPosition = new Vector2(newX, newY);
            transform.position = currentPosition;

            if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
            {
                isMoving = false;
                moveDirection = Vector2.zero;
            }
            Move(moveDirection.x);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    public void Move(float move)
    {
        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool LockPlayer()
    {
        isLock = true;
        isMoving = false;
        animator.SetBool("isMoving", false);
        InteractionEventSystem.RaiseLockStateChanged(true);
        return isLock;
    }

    public bool ReleasePlayer()
    {
        isLock = false;
        InteractionEventSystem.RaiseLockStateChanged(false);
        return isLock;
    }
}
